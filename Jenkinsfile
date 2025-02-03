pipeline {
    agent any
    
    triggers {
        pollSCM('* * * * *') 
        cron('0 0 * * *')  
    }
    
    environment {
        SOLUTION_PATH = "${WORKSPACE}/TAAdvance.sln"
        PROJECT_PATH = "${WORKSPACE}/TAAdvance/TAF/TAF.csproj"
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
        stage('List Files') {
            steps {
                script {
                    
                    def folderPath = "${WORKSPACE}/TAAdvance"

                    sh """
                        echo "Файлы в папке ${folderPath}:"
                        ls -la ${folderPath}
                    """
                }
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
                    nunit 'TestResults/**/*.xml' 
                }
            }
        }
        
        stage('Update Jira') {
            steps {
                script {
                    def xmlFile = readFile('TestResults/test-results.xml')
                    def parsedXml = new XmlSlurper().parseText(xmlFile)
            
                    def passed = parsedXml['test-case'].findAll { it.@result == 'Passed' }.size()
                    def failed = parsedXml['test-case'].findAll { it.@result == 'Failed' }.size()
            
                    jiraAddComment(
                        site: 'JIRA_SITE',
                        issueKey: 'TA-123',
                        comment: "Test results: Passed: ${passed}, Failed: ${failed}"
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
