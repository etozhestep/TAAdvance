pipeline {
    agent any
    
    triggers {
        pollSCM('* * * * *') 
        cron('0 0 * * *')  
    }
    
    environment {
        SOLUTION_PATH = "${WORKSPACE}/TAAdvance.sln"
        PROJECT_PATH = "${WORKSPACE}/TAF/TAF.csproj"
        SLACK_CHANNEL = '#ci-cd'
        JIRA_SITE = 'https://taadnvance.atlassian.net/'
        JIRA_PROJECT_KEY = 'jira-creds'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet build ${SOLUTION_PATH}'
            }
        }
        
        stage('Run Tests') {
            steps {
                sh '''
                dotnet test ${PROJECT_PATH} \
                    --results-directory TestResults \
                    --logger "trx;LogFileName=test-results.trx"
                '''
            }
            post {
                always {
                    junit 'TestResults/**/*.trx'
                }
            }
        }
        
        stage('Update Jira Issues') {
            when {
                expression { currentBuild.result == null || currentBuild.result == 'SUCCESS' }
            }
            steps {
                script {
                    def testResults = readJSON text: sh(script: 'cat TestResults/*.trx', returnStdout: true)
                    
                    jiraSendTestResults(
                        site: env.JIRA_SITE,
                        testResults: testResults,
                        issueKeys: findJiraIssues().key.join(',')
                    )
                }
            }
        }
    }
    
    post {
        always {
            reportPortalPublisher(
                endpoint: 'http://localhost:9090/',
                tokenCredentialsId: 'report-portal-token',
                launchName: 'TAAdvance Build ${env.BUILD_NUMBER}',
                logPattern: '**/*.log',
                tags: ['CI', 'TAAdvance'],
                description: 'Automated build and test run'
            )
            
            cleanWs()
        }
    }
}
