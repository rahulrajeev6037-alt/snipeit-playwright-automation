
using PlaywrightTests.Fixtures;
using PlaywrightTests.Pages;
using DotNetEnv;

namespace PlaywrightTests
{
    public class AssetCreationUserFlow : IClassFixture<PlaywrightFixture>
    {
        private readonly PlaywrightFixture _fixture;
        private const string AssetModel = "Macbook Pro 13\"";
        private const string AssetStatus = "Ready to Deploy";

        public AssetCreationUserFlow(PlaywrightFixture fixture)
        {
            _fixture = fixture;
            // Load environment variables from .env file
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            if (File.Exists(envPath))
            {
                Env.Load(envPath);
            }
        }

        [Fact]
        public async Task LoginToSnipeItApp()
        {
            await using var context = await _fixture.NewContextAsync();
            var page = await context.NewPageAsync();
            var login = new LoginPage(page);

            // Credentials from environment variables
            var username = Environment.GetEnvironmentVariable("TEST_USERNAME");
            var password = Environment.GetEnvironmentVariable("TEST_PASSWORD");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("TEST_USERNAME and TEST_PASSWORD environment variables must be set in .env file");
            }

            // Login to the Snipe-IT application and assert success
            await login.LoginToSnipeItApp(username, password);
            Assert.True(await login.IsLoggedInAsync(), "Expected to be logged in as Admin after submitting credentials.");
            
            //Create a Macbook Pro 13" asset with 'Ready to Deploy' status checked out to a random user
            var assetPage = new AssetPage(page);
            var created = await assetPage.CreateNewAssetAsync();
            Assert.True(created, "Expected the new asset to be created successfully.");

            // Verify the asset creation success message
            var verified = await assetPage.VerifyAssetCreationStatus("success");
            Assert.True(verified, $"Expected to see success message containing asset tag: {assetPage.AssetTagId}");

            // Search for the created asset in the assets list
            var found = await assetPage.searchAssetFromListAndNavigateToDetails();
            Assert.True(found, $"Expected to find asset {assetPage.AssetTagId} in the search results.");

            // Verify asset information (model and status)
            var infoVerified = await assetPage.verifyAssetInfo(AssetModel, AssetStatus);
            Assert.True(infoVerified, "Expected to verify asset model and status on asset details page.");

            // Navigate to History tab and verify asset details
            var historyNavigated = await assetPage.navigateToHistoryAndVerifyDetails();
            Assert.True(historyNavigated, "Expected to successfully navigate to History tab.");
            var historyVerified = await assetPage.verifyAssetHistory(AssetModel);
            Assert.True(historyVerified, "Expected to verify asset history details.");
        }
    }
}
