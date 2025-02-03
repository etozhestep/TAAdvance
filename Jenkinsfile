pipeline {
    agent any
    
    triggers {
        pollSCM('* * * * *') 
        cron('0 0 * * *')  
    }
    
    environment {
        SOLUTION_PATH = "${WORKSPACE}/TAAdvance.sln"
        SLACK_CHANNEL = '#ci-cd'
        JIRA_PROJECT_KEY = 'TA'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout([
                    $class: 'GitSCM',
                    branches: [[name: '*/main']],
                    extensions: [
                        [$class: 'CloneOption', depth: 0, noTags: false, shallow: false],
                        [$class: 'CleanBeforeCheckout']
                    ],
                    userRemoteConfigs: [[
                        url: 'git@github.com:etozhestep/TAAdvance.git',
                        credentialsId: 'git'
                    ]]
                ])
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet build ${SOLUTION_PATH}'
            }
        }
        
        stage('Unit Tests') {
            steps {
                sh '''
                dotnet test ${SOLUTION_PATH} \
                    --results-directory TestResults \
                    --filter "Category=Unit"
                '''
            }
            post {
                always {
                    junit 'TestResults/**/*.trx'
                    archiveArtifacts artifacts: 'TestResults/**/*', allowEmptyArchive: true
                }
            }
        }
        
        stage('UI Tests') {
            steps {
                sh '''
                dotnet test ${SOLUTION_PATH} \
                    --results-directory TestResults \
                    --filter "Category=UI"
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
