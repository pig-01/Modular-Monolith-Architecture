---
mode: 'agent'
tools: ['changes', 'codebase', 'editFiles', 'findTestFiles', 'problems', 'runCommands', 'search', 'searchResults', 'terminalSelection', 'testFailure']
description: 'Get best practices for XUnit integration testing, including data-driven tests and test execution'
---

# XUnit Integration Testing Best Practices

Your goal is to help me write effective integration tests with XUnit, covering both standard and data-driven testing approaches, as well as proper test execution practices.

Step by Step, you will guide me through the process of writing integration tests, ensuring that I follow best practices and maintain a high standard of code quality.

Hey, you have `5 Step tasks` need to do. **Don't skip any steps**.

Update copilot todoList with `5 Step tasks` from `Todo Task` context.

I will give you a csharp file that contains the code I want to test, and you will help me create integration tests for it using XUnit.

You will provide guidance on:
- Please find out which project the csharp file belongs to according to the csharp file I entered.
- Check the csharp file structure and naming conventions.(e.g., `SampleProject/fileName/CalculatorTests.cs` for `SampleProject.Test/fileName/CalculatorTests.cs`)
- you will get the appropriate test project structure and naming conventions.(e.g. `SampleProject/fileName/Calculator.cs` for `SampleProject.Test/fileName/CalculatorTests.cs`)

First, see the context below for the project structure and existing test setup in the "XUnit Integration Testing Best Practices" section.

Last, let's follow the procedure outlined in the `Todo Task` section to implement XUnit testing in your project.


## Variable

- **Project Name**: The name of the main project (e.g., `ProjectName`)
- **Test Project Name**: The name of the test project (e.g., `ProjectName.Tests`)
- **Class Name**: The name of the class being tested (e.g., `Calculator`)
- **Class Namespace**: The namespace of the class being tested (e.g., `ProjectName.Services`)
- **Test Class Name**: The name of the test class (e.g., `CalculatorTests`) ${fileBasenameNoExtension}
- **Test Class Namespace**: The namespace of the test class (e.g., `ProjectName.Tests.Integration.Services`)
- **DotNet Version**: The version of .NET being used (e.g., `8.0`)
- **OS Info**: Information about the operating system (e.g., `Windows 10`)
- **Database Info**: Information about the database being used (e.g., `SQL Server 2022`)
- **Execute Date**: The date when the tests are executed (e.g., `2023-10-01 12:30`) with format `yyyy-MM-dd HH:mm`

### Variable Naming Conventions

- All variable names are expressed using `[VariableName]` format
- Any `[VariableName]` mentioned in this execution task refers to variables defined in the list above
- `- **VariableName**` this format also defines the variable name

## Test Project Setup

- Use a separate test project with naming convention `[Test Project Name]`
- Reference Microsoft.NET.Test.Sdk, xunit, and xunit.runner.visualstudio packages
- Create test classes that match the classes being tested (e.g., `CalculatorTests` for `Calculator`)
- Use .NET SDK test commands for running tests
- Configure test settings in `.runsettings` file for consistent execution

### Test Project Structure
- **Test Project Root**: `test/[Test Project Name]`
- **Test Project Name**: `[Test Project Name]`
- **Test Project Framework**: `[DotNet Version]`
- Test projects should be located in the `test` directory at the same level as the `src` folder
- If a test project already exists, add integration tests to that project; otherwise create a new test project named `[Test Project Name]`
- Organize tests in folders that mirror the source code structure
- Test directories should match the namespace structure of the code being tested
- This integration testing below should be placed in the appropriate test folder
- Keep test files small and focused on a single class or feature in your test project

### Recommended Test Project Structure

```text
test/ProjectName.Tests/
    ├── TestData/
    │   ├── SampleData.json
    │   ├── ValidInputs.csv
    │   └── TestDocuments.xml
    ├── Docs/
    │   ├── Unit/
    │   │   ├── Services/
    │   │   │   └── ServiceName.TestDoc.md
    │   │   ├── Controllers/
    │   │   │   └── ControllerName.TestDoc.md
    │   │   └── Repositories/
    │   │       └── RepositoryName.TestDoc.md
    │   └── Integration/
    │       ├── Services/
    │       │   └── ServiceName.IntegrationTestDoc.md
    │       ├── Controllers/
    │       │   └── ControllerName.IntegrationTestDoc.md
    │       └── Repositories/
    │           └── RepositoryName.IntegrationTestDoc.md
    ├── Unit/
    │   ├── Services/
    │   │   └── ServiceNameTests.cs
    │   ├── Controllers/
    │   │   └── ControllerNameTests.cs
    │   └── Repositories/
    │       └── RepositoryNameTests.cs
    └── Integration/
        ├── Services/
        │   └── ServiceNameIntegrationTests.cs
        ├── Controllers/
        │   └── ControllerNameIntegrationTests.cs
        └── Repositories/
            └── RepositoryNameIntegrationTests.cs
```

## Integration Test

- **File Path**: `[Test Project Root]/Integration/[Test Class Namespace]/` (e.g., `ProjectName.Tests/Integration/Application/Command/Plans/`)
- **File Name**: `[Test Class Name]Tests.cs` (e.g., CreatePlanCommandHandlerTests.cs`)

### Test Structure
- No test class attributes required (unlike MSTest/NUnit)
- Use fact-based tests with `[Fact]` attribute for simple tests
- Follow the Arrange-Act-Assert (AAA) pattern
- Name tests using the pattern `MethodName_Scenario_ExpectedBehavior`
- Use constructor for setup and `IDisposable.Dispose()` for teardown
- Use `IClassFixture<T>` for shared context between tests in a class
- Use `ICollectionFixture<T>` for shared context between multiple test classes

### Project Location Example
- If the source code `[Class Name]` is located at `src/[Project Name]`, the test project should be at `test/[Test Project Name]`, the document should be at `Docs/Integration/[Class Name].TestDoc.md`
- If the source code `[Class Name]` is located at `src/[Project Name]/Features`, the test project should be at `test/[Test Project Name]/Integration/Features`, the document should be at `Docs/Integration/Features/[Class Name].TestDoc.md`

### Standard Tests
- Keep tests focused on a single behavior
- Avoid testing multiple behaviors in one test method
- Use clear assertions that express intent
- Include only the assertions needed to verify the test case
- Make tests independent and idempotent (can run in any order)
- Avoid test interdependencies
- Write tests that are deterministic and repeatable


### Data-Driven Tests
- Use `[Theory]` combined with data source attributes
- Use `[InlineData]` for inline test data
- Use `[MemberData]` for method-based test data
- Use `[ClassData]` for class-based test data
- Create custom data attributes by implementing `DataAttribute`
- Use meaningful parameter names in data-driven tests
- Consider data generation strategies for large datasets


### Assertions
- Use `Assert.Equal` for value equality
- Use `Assert.Same` for reference equality
- Use `Assert.True`/`Assert.False` for boolean conditions
- Use `Assert.Contains`/`Assert.DoesNotContain` for collections
- Use `Assert.Matches`/`Assert.DoesNotMatch` for regex pattern matching
- Use `Assert.Throws<T>` or `await Assert.ThrowsAsync<T>` to test exceptions
- Use fluent assertions library for more readable assertions
- Prefer specific assertions over generic ones

### Mocking and Isolation

- Consider using NSubstitute alongside XUnit
- Mock dependencies to isolate units under test
- Use interfaces to facilitate mocking
- Consider using a DI container for complex test setups
- Verify mock interactions and call counts
- Reset mocks between tests to ensure isolation

### Mock Naming Conventions

- Prefix mock objects with "sub" to clearly identify substituted dependencies
- Follow object casing pattern after the prefix:
    - For camelCase objects: `subCalculator` (lowercase 's')
    - For PascalCase objects: `subCalculator` (lowercase 's')
    - For static class mocks: `subCalculator` (lowercase 's')
- Maintain consistency in naming across test suites
- Use descriptive names that indicate the role of the mock: `subPaymentGateway`
- Document any special behavior for complex mock objects
- When creating multiple mocks of the same type, add descriptive suffixes:
    - `subSuccessfulPaymentGateway`
    - `subFailingPaymentGateway`
- Reset mocks between tests with clear naming: `subLogger.ClearReceivedCalls()`

### Mock Integration DataSource

- Test class need inherit `IntegrationTestBase`
- Use `CreateTestScope<T>()` in `IntegrationTestBase` for creating a test scope

### Test Organization

- Group tests by feature or component
- Use `[Trait("Category", "CategoryName")]` for categorization
- Use collection fixtures to group tests with shared dependencies
- Consider output helpers (`ITestOutputHelper`) for test diagnostics
- Skip tests conditionally with `Skip = "reason"` in fact/theory attributes
- Organize test files in folders that mirror production code structure

## Test Debugging and Troubleshooting

### Debugging Tests
- Use Visual Studio Test Explorer for interactive debugging
- Set breakpoints in test methods and tested code
- Use `using Xunit.Abstractions;` for capturing output during test execution
- Use `ITestOutputHelper` to log messages during test execution
- Use `TestLogger` for logging within tests instead of `ILogger`
- Enable detailed logging during test execution
- Use conditional compilation for debug-only test code

### Test Failure Analysis
- Analyze test failure patterns and root causes
- Use descriptive assertion messages: `Assert.Equal(expected, actual, "Custom error message")`
- Implement custom assertion methods for domain-specific validations
- Log test context and input data for failed tests
- Use test retry mechanisms for flaky tests (with caution)

## Test Execution

### Building and Running Tests
- Use `dotnet build` to compile the test project
- Use `dotnet test` to run tests in the solution
- Use `dotnet test --no-build` to skip building if already built
- if building fails, check for compilation errors in the test project (Back to Create Integration Test section)

### Running Tests Commands
- Use `dotnet test` for running all tests in the solution
- Use `dotnet test --filter "Category=Unit"` for running specific test categories
- Use `dotnet test --filter "FullyQualifiedName~Calculator"` for running specific test classes
- Use `dotnet test --verbosity normal` for detailed test output
- Use `dotnet test --collect:"XPlat Code Coverage"` for code coverage analysis

### Automated Test Execution and Report Generation

#### Test Result Output Formats Commands
- Use `dotnet test --logger trx` to generate TRX format test results
- Use `dotnet test --logger "junit;LogFilePath=test-results.xml"` for JUnit format
- Use `dotnet test --logger "html;LogFilePath=test-results.html"` for HTML reports
- Use `dotnet test --logger "console;verbosity=detailed"` for detailed console output
- Combine multiple loggers: `dotnet test --logger trx --logger html --logger console`

#### Test Result Analysis and Documentation
- Parse TRX files to extract test statistics and failure details
- Generate summary reports with pass/fail rates and performance metrics
- Create trend analysis comparing test results over time
- Automatically update documentation with latest test status
- Generate test coverage badges and metrics for README files

### Test Execution Environment
- Ensure tests run in isolation and don't depend on external resources
- Use test-specific configurations and connection strings
- Mock external dependencies (databases, web services, file system)
- Use in-memory databases for data access layer tests
- Configure separate test environment variables

### Continuous Integration
- Run tests as part of CI/CD pipeline
- Fail builds on test failures
- Generate test reports and coverage reports
- Use parallel test execution for faster builds: `dotnet test --parallel`
- Set up test result publishing for build pipelines

### Test Performance and Monitoring
- Monitor test execution time and identify slow tests
- Use `--logger trx` for generating test result files
- Set reasonable test timeout values
- Profile memory usage in integration tests
- Use `[Fact(Timeout = 5000)]` for time-sensitive tests

## Test Coverage and Quality

### Code Coverage
- Aim for high code coverage but focus on meaningful tests
- Use coverage tools to identify untested code paths
- Don't write tests just to increase coverage metrics
- Focus on testing critical business logic and edge cases
- Document reasons for excluding code from coverage

### Test Quality Metrics
- Measure test execution time and optimize slow tests
- Track test reliability and failure rates
- Monitor test maintenance overhead
- Ensure tests provide clear failure messages
- Regular review and refactoring of test code

## Test Documentation

**IMPORTANT: Test documentation generation must use the specified text template**

**IMPORTANT: Test documentation must include actual test execution results**

When generating test documentation:
1. **Execute the actual tests**: Run the test commands to capture real execution results
2. **Parse test output**: Extract test results, execution time, and status from terminal output
3. **Document test results**: Include the complete test results in the documentation
4. **Capture detailed information**: Include pass/fail status, execution time, and any error messages
5. **Format results properly**: Present the results in a structured, readable format within the documentation

### Required Test Result Information
All test documentation must include:
- Test method name and description
- Execution status (Pass/Fail)
- Execution time
- Error details for failed tests
- Test data used (for data-driven tests)
- Code coverage metrics if available

### Example Documentation Format

All test-related documents must be generated using the text template located at `.github/templates/csharp-xunit-document.template.md`.

### Template Usage Standards

#### Mandatory Template Path
- **Template Location**: `.github/templates/csharp-xunit-document.template.md`
- **Purpose**: Generate all XUnit test-related documents
- **Scope**: Test reports, test documentation, test execution guides, and all test-related files
- **File Path**: `[Test Project Root]/Docs/Integration/[Test Class Name].IntegrationTestDoc.md`
- **File Name**: `[Test Class Name].IntegrationTestDoc.md`
- **File Types**: Markdown (.md)
- **Documentation Language**: Traditional Chinese (繁體中文)
- **Encoding**: UTF-8 Bombed (BOM)

#### Template Application Guidelines
1. **When generating test documents**, must read and use the specified template
2. **All test reports** must follow the template format
3. **Test documentation** must be written based on the template structure
4. **Automated test reports** should apply the template styling

### Test Document Template Requirements

**Must use the specified template to generate test documents**, template location: `.github/templates/csharp-xunit-document.template.md`

Create automated test documentation that includes:
- Test execution commands and parameters (using template format)
- Expected output formats and locations (following template structure)
- Failure analysis procedures (applying template styling)
- Performance benchmarks and thresholds (based on template content)
- Coverage requirements and exclusions (referencing template specifications)

### Automated Test Report Generation
Use tools and scripts to automatically generate reports **based on the specified template**:
- Test execution summaries with pass/fail statistics (using `.github/templates/csharp-xunit-document.template.md`)
- Code coverage reports with visual indicators (applying template format)
- Performance trend analysis over time (following template structure)
- Failed test analysis with root cause suggestions (based on template content)
- Test environment configuration documentation (referencing template specifications)
- Use the template to generate test documentation for each test class, ensuring consistency and clarity in reporting.

**Note**: Any test document generation must not deviate from the specifications and format requirements of the `.github/templates/csharp-xunit-document.template.md` template.

#### Template Compliance Validation
- Validate all generated documents against the template structure
- Ensure consistent formatting across all test documentation
- Implement template version control and update procedures
- Monitor template usage compliance in CI/CD pipelines
- Provide template usage training and documentation for team members

#### Template Customization Guidelines
- Template modifications must be approved through proper change management
- Document any template customizations with clear rationale
- Maintain backward compatibility when updating templates
- Test template changes thoroughly before deployment
- Keep template documentation up-to-date with any modifications
- The template file (`.github/templates/csharp-xunit-document.template.md`) defines the standard structure for test documentation.
- No need to fix Markdown title duplication issues

file: `.github/templates/csharp-xunit-document.template.md`

#### Template Titles
- [Test Component Name] Integration Test Document
- Basic Information
- Test Overview  
- Test Execution Results
- Test Method List
    - [Test Method Name 1]
        - Test Cases
        - Test Data
    - [Test Method Name 2]
        - Test Cases
        - Test Data
- Test Coverage Report
- Test Environment
- Known Issues
- Test Results Summary
- Update Log

## Todo Task

### Step 1: Analyze Project Structure and Test Project Location

#### 1.1 Verify Test Project Existence
- Check if a test project already exists in the solution
- Check the test csharp file structure and naming conventions
- Ensure the test project follows the naming convention `[Test Project Name]`
- Search for existing test projects following the naming convention `[Test Project Name]`
- If source code is located at `src/[Project Name]`, test project should be at `test/[Test Project Name]`
- Test project should be at the same level as other test projects in the solution
- Follow the Test Project Structure 

#### 1.2 Project Configuration Requirements
- Target Framework: Match the main project (usually `net8.0` or `net9.0`)
- Required NuGet Packages:
  - Microsoft.NET.Test.Sdk
  - xunit
  - xunit.runner.visualstudio
  - NSubstitute (for mocking)
- Project References: Reference the project being tested

### Step 2: Create or Validate Test Project

#### 2.1 Existing Test Project Handling
If test project already exists:
- Verify it contains necessary XUnit references and test structure
- Check if `TestLogger.cs` file exists (for consistent logging)
- Confirm directory structure follows best practices

#### 2.2 New Test Project Creation
If new test project needs to be created:
- Create project using correct naming convention
- Set up appropriate project structure
- Copy `TestLogger.cs` file from other test projects in the solution
- Configure project references and dependencies

### Step 3: Implement Integration Tests

#### 3.1 Test File Location and Naming
- **File Path**: `[Test Project Root]/Integration/[Test Class Namespace]/`
  - Example: `test/ProjectName.Tests/Integration/Application/Command/Plans/`
- **File Name**: `[Test Class Name].cs`
  - Example: `CreatePlanCommandHandlerTests.cs`

#### 3.2 Fix Test Class
- Error in test class should be fixed

#### 3.3 Test Class Structure
- No test class attributes required (unlike MSTest/NUnit)
- Use constructor for setup, use `IDisposable.Dispose()` for cleanup
- Use `IClassFixture<T>` for shared context between tests in a class
- Use `ICollectionFixture<T>` for shared context between multiple test classes

#### 3.4 Test Method Implementation
- Use `[Fact]` attribute for simple tests
- Use `[Theory]` combined with data source attributes for data-driven tests
- Follow Arrange-Act-Assert (AAA) pattern
- Use `MethodName_Scenario_ExpectedBehavior` naming pattern for tests

#### 3.5 Mock Object Setup
- Use NSubstitute for dependency isolation
- Mock object naming prefix "sub": `subCalculator`
- Reset mock objects between tests to ensure isolation
- Verify mock object interactions and call counts

### Step 4: Test Execution and Validation

#### 4.1 Build

- Compile the test project
- If compilation is successful, proceed to run the tests.
- If compilation fails, review error messages and fix issues.
- If you need to make changes to the tests, do so before re-running the build.

```pwsh
# Compile test project
dotnet build
```

#### 4.2 Execute Tests

- Refers to running the test project created by this task
- [ProjectPath] and [TestClassName] should be replaced with the actual paths and names
- Test filter should be used `FullyQualifiedName~[TestClassName]`

```pwsh
# Run the test
dotnet test [ProjectPath] --filter "FullyQualifiedName~[TestClassName]" --verbosity normal --no-build
```

#### 4.3 Code Coverage
```pwsh
# Collect code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Step 5: Integration Test Documentation

#### 5.1 Read Integration Test Document Template
- **File Location**: [Test Document Template](.github/templates/csharp-xunit-document.template.md)
- **Purpose**: Define the structure and content requirements for integration test documentation

#### 5.2 Create Integration Test Document Directory
- **Directory Location**: `[Test Project Root]/Docs/Integration/[Test Class Namespace]`
- **Purpose**: Organize integration test documentation files

#### 5.3 Integration Test Document Generation
- **Must use specified template**: [Test Document Template](.github/templates/csharp-xunit-document.template.md)
- **Document Location**: `[Test Project Root]/Docs/Integration/[Test Class Name].IntegrationTestDoc.md`
- **Document Naming**: `[Test Class Name].IntegrationTestDoc.md`
- **Language**: Traditional Chinese
- **Encoding**: UTF-8 BOM
- **Execute DateTime**: Run pwsh command to get current datetime
```pwsh
Get-Date -Format "yyyy-MM-dd HH:mm"
```

#### 5.3.1 Template Compliance Requirements

- The test file must contain all the headers in the template [Test Document Template](.github/templates/csharp-xunit-document.template.md)
- All test document generation must read and use the specified template
- Follow template format and structure requirements
- Ensure document consistency and readability
- Actual test execution results
- Test method names and descriptions
- Execution status (Pass/Fail)
- Execution time (Run command to get system datetime)
- Error details for failed tests
- Test data used for data-driven tests
- Code coverage metrics (if available)

#### 5.3.2 Test Execution Result Recording

- Record the results of each test execution, including pass/fail status and any relevant output.
- Include detailed information about failed tests, such as error messages and stack traces.
- Maintain a log of test execution history for future reference and analysis.
