using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlaywrightTests.Fixtures
{
	public class PlaywrightFixture : IAsyncLifetime
	{
		public IPlaywright? Playwright { get; private set; }
		public IBrowser? Browser { get; private set; }
		public string BaseUrl { get; }

		public PlaywrightFixture()
		{
			BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "https://demo.snipeitapp.com";
		}

		public async Task InitializeAsync()
		{
			Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
			var headlessEnv = Environment.GetEnvironmentVariable("HEADLESS");
			var headless = string.IsNullOrEmpty(headlessEnv) ? false : !headlessEnv.Equals("false", StringComparison.OrdinalIgnoreCase);
			Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
			{
				Headless = headless
			});
		}

		public async Task DisposeAsync()
		{
			if (Browser != null)
				await Browser.DisposeAsync();
			Playwright?.Dispose();
		}

		public async Task<IBrowserContext> NewContextAsync()
		{
			if (Browser == null)
				throw new InvalidOperationException("Browser has not been initialized. Ensure InitializeAsync has run.");

			return await Browser.NewContextAsync(new BrowserNewContextOptions
			{
				BaseURL = BaseUrl
			});
		}
	}
}

