pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build TAAdvance.sln'
            }
        }

        stage('Tests') {
            steps {
                sh 'dotnet test TAAdvance.sln --logger "trx;LogFileName=test-results.trx"'
            }
            post {
                always {
                    junit '**/*.trx'
                }
            }
        }
    }
}
