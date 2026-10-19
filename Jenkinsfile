// CI/CD в Jenkins: сборка, API-тесты, UI-тесты в выбранных браузерах, Allure-отчёт.
// Нужны плагины: Docker Pipeline, Allure Jenkins Plugin (+ Allure Commandline в Global Tool Configuration).
pipeline {
    agent {
        dockerfile {
            filename 'Dockerfile'
            args '--shm-size=2g -e HOME=/tmp'
        }
    }

    parameters {
        choice(name: 'SUITE', choices: ['all', 'smoke', 'api', 'ui'], description: 'Набор тестов')
        choice(name: 'BROWSERS', choices: ['Chrome', 'Firefox', 'Chrome Firefox'],
               description: 'Браузеры для UI-тестов (Chrome Firefox — кросс-браузерный прогон)')
        string(name: 'THREADS', defaultValue: '4', description: 'Количество параллельных потоков')
    }

    triggers {
        cron('H 5 * * 1-5') // ночная регрессия по будням
    }

    options {
        timestamps()
        timeout(time: 60, unit: 'MINUTES')
    }

    environment {
        RESULTS_DIR = 'Jefit.Test/bin/Release/net8.0/allure-results'
        // Тестовый аккаунт для @auth: создайте credentials типа "Username with password" с id jefit-test-account
        // и раскомментируйте строку ниже. Jenkins создаст переменные JEFIT_ACCOUNT_USR и JEFIT_ACCOUNT_PSW.
        // JEFIT_ACCOUNT = credentials('jefit-test-account')
    }

    stages {
        stage('Build') {
            steps {
                sh 'rm -rf allure-results && mkdir -p allure-results'
                sh 'dotnet build JefitAutomation.sln -c Release'
            }
        }

        stage('API tests') {
            when { expression { params.SUITE in ['all', 'smoke', 'api'] } }
            steps {
                catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
                    sh """
                        FILTER="Category=api"
                        [ "${params.SUITE}" = "smoke" ] && FILTER="\$FILTER&Category=smoke"
                        dotnet test Jefit.Test -c Release --no-build \
                            --filter "\$FILTER" -- NUnit.NumberOfTestWorkers=${params.THREADS}
                    """
                }
                sh 'cp -r $RESULTS_DIR/. allure-results/ || true'
            }
        }

        stage('UI tests') {
            when { expression { params.SUITE in ['all', 'smoke', 'ui'] } }
            steps {
                script {
                    // Браузеры запускаются по очереди, результаты копируются в общую папку для одного отчёта
                    for (browser in params.BROWSERS.split(' ')) {
                        stage("UI: ${browser}") {
                            catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
                                withEnv(["JEFIT_TestSettings__BrowserType=${browser}"]) {
                                    sh """
                                        export JEFIT_TestSettings__UserEmail="\${JEFIT_ACCOUNT_USR:-}"
                                        export JEFIT_TestSettings__UserPassword="\${JEFIT_ACCOUNT_PSW:-}"
                                        FILTER="Category=ui"
                                        [ "${params.SUITE}" = "smoke" ] && FILTER="\$FILTER&Category=smoke"
                                        dotnet test Jefit.Test -c Release --no-build \
                                            --filter "\$FILTER" -- NUnit.NumberOfTestWorkers=${params.THREADS}
                                    """
                                }
                            }
                            sh 'cp -r $RESULTS_DIR/. allure-results/ || true'
                        }
                    }
                }
            }
        }
    }

    post {
        always {
            sh "echo 'Browsers=${params.BROWSERS}' >> allure-results/environment.properties || true"
            allure includeProperties: false, results: [[path: 'allure-results']]
            archiveArtifacts artifacts: 'allure-results/**', allowEmptyArchive: true
        }
    }
}
