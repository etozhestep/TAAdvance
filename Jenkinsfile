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
                '''
            }
            post {
                always {
                    junit 'TestResults/**/*.trx'
                    script {
                        def testResults = readJSON text: sh(script: 'cat TestResults/*.trx', returnStdout: true)
                        jiraUpdateIssue site: 'JIRA_SITE',
                            issueKey: "${JIRA_PROJECT_KEY}-${env.BUILD_NUMBER}",
                            testResults: testResults
                    }
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
