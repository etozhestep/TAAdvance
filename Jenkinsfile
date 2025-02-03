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
        JIRA_PROJECT_KEY = 'TA'
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
                        --logger "nunit;LogFileName=TestResults/test-results.xml"
                '''
            }
            post {
                always {
                    nunit testResultsPattern: 'TestResults/**/*.xml' 
                }
            }
        }
        
        stage('List Files') {
            steps {
                script {
                    def folderPath = "${WORKSPACE}/TAAdvance"
                    sh """
                        echo "Files ${folderPath}:"
                        ls -la ${folderPath}
                    """
                }
            }
        }
        
        stage('Update Jira') {
            steps {
                script {
                    def xmlFile = readFile('TestResults/test-results.xml')
                    def parsedXml = new XmlSlurper().parseText(xmlFile)
            
                    def passed = parsedXml.'test-suite'.'test-case'.findAll { it.@result == 'Passed' }.size()
                    def failed = parsedXml.'test-suite'.'test-case'.findAll { it.@result == 'Failed' }.size()
            
                    jiraAddComment(
                        site: env.JIRA_SITE,
                        issueKey: "${env.JIRA_PROJECT_KEY}-${env.BUILD_NUMBER}",
                        comment: "Test results: passed: ${passed}, failed: ${failed}"
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
                launchName: "TAAdvance Build ${env.BUILD_NUMBER}",
                logPattern: '**/*.log',
                tags: ['CI', 'TAAdvance']
            )
            cleanWs()
        }
    }
}
