# Snipe-IT Automation Test Suite

## Project Description

This is an automated test suite for the Snipe-IT asset management application using Playwright for .NET. The project demonstrates end-to-end testing capabilities for web applications.

The test suite validates the complete asset creation workflow in the Snipe-IT demo application, from user authentication through asset creation, verification, and history validation.

## Test Scenario

The automated test performs the following steps:

1. **Login** - Authenticates to the Snipe-IT demo application at https://demo.snipeitapp.com/login
2. **Create Asset** - Creates a new Macbook Pro 13" asset with "Ready to Deploy" status, checked out to a random user
3. **Verify Creation** - Confirms the asset creation success message is displayed
4. **Search Asset** - Finds the newly created asset in the assets list
5. **Navigate to Details** - Opens the asset details page from the search results
6. **Validate Details** - Verifies asset information (model, status, category) on the details page
7. **Check History** - Navigates to the History tab and validates asset creation history entries

## Prerequisites

Before running the tests, ensure you have the following installed:

- **.NET 10.0 SDK** or later
  - Download from: https://dotnet.microsoft.com/download
  - Verify installation: `dotnet --version`

- **Playwright Browsers**
  - Will be installed during setup (see Installation section)

- **Git** (optional, for cloning the repository)

## Installation

Follow these steps to set up the project:

### 1. Clone or Download the Project

```bash
git clone <repository-url>
cd PlaywrightTests
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Install Playwright Browsers

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

Or on non-Windows systems:
```bash
dotnet build
playwright install
```

### 4. Configure Environment Variables

Create a `.env` file in the project root directory:

```bash
# Copy the example file
cp .env.example .env
```

Edit the `.env` file and add your Snipe-IT demo credentials:

```env
TEST_USERNAME=your_username_here
TEST_PASSWORD=your_password_here
```

**Note:** For the Snipe-IT demo environment, use the credentials provided by the demo site.

## How to Run Tests

### Run All Tests

```bash
dotnet test
```

### Run Tests with Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run Tests in Headless Mode

By default, tests run in headed mode (browser visible). To run headless:

```bash
# Set environment variable (Windows PowerShell)
$env:HEADLESS="true"
dotnet test

# Or in Command Prompt
set HEADLESS=true
dotnet test

# Or on Linux/Mac
HEADLESS=true dotnet test
```

### Run Tests and View Browser

```bash
# Ensure HEADLESS is false or unset (Windows PowerShell)
$env:HEADLESS="false"
dotnet test
```

### Build the Project

```bash
dotnet build
```

## Project Structure

```
PlaywrightTests/
├── Pages/
│   ├── LoginPage.cs           # Login page object model
│   └── AssetPage.cs            # Asset management page object model
├── Fixtures/
│   └── PlaywrightFixture.cs   # Browser lifecycle management
├── AssetCreationUserFlow.cs   # Main test class
├── PlaywrightTests.csproj     # Project configuration
├── .env                        # Environment variables (not in git)
├── .env.example               # Environment variables template
├── .gitignore                 # Git ignore rules
└── README.md                  # This file
```

## Architecture & Design Patterns

### Page Object Model (POM)
- Encapsulates page-specific logic in dedicated classes
- Promotes code reusability and maintainability
- Separates test logic from page implementation details

### Fixture Pattern
- `PlaywrightFixture` manages browser lifecycle using XUnit's `IClassFixture`
- Ensures proper initialization and cleanup
- Supports parallel test execution

### Environment Configuration
- Credentials stored in `.env` file (excluded from version control)
- Configurable headless/headed mode
- Base URL configuration support

## Key Features

- ✅ Robust error handling with specific exception types
- ✅ Comprehensive logging for debugging
- ✅ XML documentation on all public methods
- ✅ Explicit waits with timeouts
- ✅ Clear assertion messages
- ✅ Secure credential management
- ✅ Page Object Model architecture

## Known Limitations

1. **No Test Cleanup**
   - Created assets are not deleted after test execution
   - Assets remain in the demo environment
   - Future enhancement: Implement teardown to delete test data

2. **Single Test Scenario**
   - Currently only covers the happy path
   - No negative test cases (invalid login, validation errors, etc.)
   - Future enhancement: Add edge cases and error scenarios

3. **Hardcoded Test Data**
   - Asset model and status are constants
   - User assignment is random (first available option)
   - Future enhancement: Implement data-driven testing with multiple test cases

4. **Demo Environment Dependency**
   - Tests rely on demo.snipeitapp.com availability
   - Demo data may change or reset periodically
   - No control over demo environment state

5. **No Parallel Execution**
   - Test creates assets with auto-generated IDs
   - Running multiple instances may cause conflicts
   - Future enhancement: Implement unique identifiers per test run

## Technologies Used

- **Playwright for .NET** (v1.57.0) - Browser automation
- **XUnit** (v2.9.3) - Test framework
- **DotNetEnv** (v3.1.1) - Environment variable management
- **.NET 10.0** - Runtime framework

## Troubleshooting

### Tests Fail with "Browser not found"
Run the Playwright browser installation command:
```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### Tests Fail with "Environment variables not set"
Ensure `.env` file exists and contains valid credentials:
```env
TEST_USERNAME=your_username
TEST_PASSWORD=your_password
```

### Timeout Errors
- Check internet connectivity
- Verify demo.snipeitapp.com is accessible
- Increase timeout values if needed (default: 60000ms)

### Selector Not Found Errors
- Demo site HTML may have changed
- Check error logs for specific selector failures
- Update selectors in Page Object files if needed

## Contributing

This is a coding assessment project. For any questions or issues, please contact the project maintainer.

## License

This project is for educational and assessment purposes only.
