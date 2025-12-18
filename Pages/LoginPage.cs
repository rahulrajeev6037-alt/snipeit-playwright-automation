using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;
        private const string BaseUrl = "https://demo.snipeitapp.com";
        private const string UsernameSelector = "xpath=//*[@id='username']";
        private const string PasswordSelector = "xpath=//*[@id='password']";
        private const string SubmitSelectors = "xpath=//*[@id='submit']";
        private const string LogoSelector = "xpath=//img[@id='login-logo']";
        private const string AdminIndicatorSelector = "xpath=//span[contains(text(), 'Admin')]";

        public LoginPage(IPage page) => _page = page;

        /// <summary>
        /// Logs into the Snipe-IT application using provided credentials.
        /// Handles navigation and authentication flow including potential redirects.
        /// </summary>
        /// <param name="username">The username for authentication</param>
        /// <param name="password">The password for authentication</param>
        /// <returns>True if login was successful, false otherwise</returns>
        public async Task<bool> LoginToSnipeItApp(string username, string password)
        {
            try
            {
                var demoLoginUrl = $"{BaseUrl}/login";

                await _page.GotoAsync(demoLoginUrl);
                await _page.WaitForURLAsync("**/login");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                const string secondarySelector = "a.button.button-secondary";
                var secondary = _page.Locator(secondarySelector);
                if (await secondary.CountAsync() > 0)
                {
                    await secondary.First.ClickAsync();

                    try
                    {
                        await _page.WaitForURLAsync("https://snipeitapp.com/login", new PageWaitForURLOptions { Timeout = 10000 });
                    }
                    catch (TimeoutException)
                    {
                        // Expected timeout if redirect doesn't happen, fallback to direct navigation
                        if (!_page.Url.Contains("snipeitapp.com/login"))
                        {
                            await _page.GotoAsync("https://snipeitapp.com/login");
                            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                        }
                    }
                }
                else
                {
                    if (!_page.Url.Contains("snipeitapp.com/login"))
                    {
                        await _page.GotoAsync("https://snipeitapp.com/login");
                        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    }
                }

                await _page.WaitForSelectorAsync(LogoSelector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
                await _page.WaitForSelectorAsync(UsernameSelector);

                await _page.FillAsync(UsernameSelector, username);
                await _page.FillAsync(PasswordSelector, password);

                var locator = _page.Locator(SubmitSelectors);
                if (await locator.CountAsync() > 0)
                {
                    await locator.First.ClickAsync();
                }
                else
                {
                    await _page.PressAsync(PasswordSelector, "Enter");
                }

                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            }
            catch (TimeoutException ex)
            {
                Console.Error.WriteLine($"Timeout error in LoginToSnipeItApp: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in LoginToSnipeItApp: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Verifies if the user is currently logged in by checking for the Admin indicator.
        /// </summary>
        /// <returns>True if logged in successfully, false otherwise</returns>
        public async Task<bool> IsLoggedInAsync()
        {
            try
            {
                var locator = _page.Locator(AdminIndicatorSelector);
                return await locator.CountAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in IsLoggedInAsync: {ex.Message}");
                return false;
            }
        }
    }
}
