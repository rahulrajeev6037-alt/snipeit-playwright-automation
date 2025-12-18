using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace PlaywrightTests.Pages
{
    public class AssetPage
    {
        private readonly IPage _page;
        private const string BaseUrl = "https://demo.snipeitapp.com";

        public string AssetTagId { get; private set; } = string.Empty;

        private const string NewAssetUrlPattern = "{0}/hardware/create";
        private const string CreateHardwareLinkSelector = "a[href=\"https://demo.snipeitapp.com/hardware/create\"]";
        private const string AssetTagInputSelector = "input#asset_tag";
        private const string ModelDropdownSelector = "id=select2-model_select_id-container";
        private const string ModelSearchFieldSelector = "input.select2-search__field";
        private const string ModelOptionSelector = "//div[contains(text(), 'Laptops - Macbook Pro 13')]";
        private const string StatusDropdownSelector = "id=select2-status_select_id-container";
        private const string StatusOptionSelector = "//li[text()='Ready to Deploy']";
        private const string LabelVisibilitySelector = "label.btn.btn-default.active";
        private const string AssignedUserDropdownSelector = "id=select2-assigned_user_select-container";
        private const string AssignedUserOptionSelector = "//*[@class='clearfix']";
        private const string SaveButtonSelector = "button.btn.btn-success.pull-right";
        private const string SuccessAlertXpathTemplate = "//div[contains(@class, 'alert-success') and contains(., '{0}')]";
        private const string SearchInputSelector = "//input[@class='form-control search-input']";
        private const string SearchResultMarkTemplate = "//mark[contains(text(), '{0}')]";
        private const string AssetTagSpanTemplate = "//span[text()='{0}']";
        private const string StatusSelectorTemplate = "//a[normalize-space()='{0}']";
        private const string CategoryLaptopsSelector = "//a[text()='Laptops']";
        private const string ModelSelectorTemplate = "//a[text()='{0}']";
        private const string HistoryTabSelector = "//a[contains(@href, '#history')]//span[@class='hidden-xs hidden-sm']";
        private const string CreateNewHistorySelector = "//td[text()='create new']";
        private const string AssetHistoryLinkTemplate = "//a[normalize-space()='#{0} - {1}']";
       
        public AssetPage(IPage page) => _page = page;

        /// <summary>
        /// Creates a new asset in the Snipe-IT application.
        /// Navigates to the asset creation page, fills in required fields including model, status, and assigned user.
        /// Stores the generated AssetTagId for later verification.
        /// </summary>
        /// <returns>True if asset creation was successful, false otherwise</returns>
        public async Task<bool> CreateNewAssetAsync()
        {
            try
            {
                var url = string.Format(NewAssetUrlPattern, BaseUrl);

                // Prefer navigation via UI: open the dropdown and click the create link if present
                var dropdown = _page.Locator("strong.caret");
                if (await dropdown.CountAsync() > 0)
                {
                    await dropdown.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    var createLink = _page.Locator(CreateHardwareLinkSelector);
                    if (await createLink.CountAsync() > 0)
                    {
                        await createLink.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    }
                    else
                    {
                        await _page.GotoAsync(url);
                    }
                }
                else
                {
                    await _page.GotoAsync(url);
                }

                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Locate the input field and get the value
                var assetTag = _page.Locator(AssetTagInputSelector);
                AssetTagId = await assetTag.InputValueAsync();

                // Click on Model dropdown
                var modelDropdown = _page.Locator(ModelDropdownSelector);
                if (await modelDropdown.CountAsync() > 0)
                {
                    await modelDropdown.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    
                    // Enter search text into the model search box
                    var modelSearch = _page.Locator(ModelSearchFieldSelector);
                    if (await modelSearch.CountAsync() > 0)
                    {
                        await modelSearch.First.FillAsync("Macbook");
                        await _page.WaitForSelectorAsync(ModelOptionSelector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 60000 });
                    }
                    
                    // Select the Laptops - Macbook Pro 13 option
                    var modelOption = _page.Locator(ModelOptionSelector);
                    if (await modelOption.CountAsync() > 0)
                    {
                        await modelOption.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    }
                }

                // Click on Status dropdown
                var statusDropdown = _page.Locator(StatusDropdownSelector);
                if (await statusDropdown.CountAsync() > 0)
                {
                    await statusDropdown.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    
                    // Select Ready to Deploy option
                    var statusOption = _page.Locator(StatusOptionSelector);
                    if (await statusOption.CountAsync() > 0)
                    {
                        await statusOption.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    }
                }

                // Wait for the label to be visible
                await _page.WaitForSelectorAsync(LabelVisibilitySelector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 60000 });

                // Click on Assigned User dropdown
                var assignedUserDropdown = _page.Locator(AssignedUserDropdownSelector);
                if (await assignedUserDropdown.CountAsync() > 0)
                {
                    await assignedUserDropdown.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    
                    // Select the clearfix option (assigned user)
                    var assignedUserOption = _page.Locator(AssignedUserOptionSelector);
                    if (await assignedUserOption.CountAsync() > 0)
                    {
                        await assignedUserOption.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    }
                }

                // Click the Save button
                var saveButton = _page.Locator(SaveButtonSelector);
                if (await saveButton.CountAsync() > 0)
                {
                    await saveButton.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }

                return true;
            }
            catch (TimeoutException ex)
            {
                Console.Error.WriteLine($"Timeout error in CreateNewAssetAsync: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in CreateNewAssetAsync: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Verifies that the asset creation success message is displayed.
        /// Checks for a success alert containing the AssetTagId.
        /// </summary>
        /// <param name="success">The expected success indicator (typically "success")</param>
        /// <returns>True if success message is found, false otherwise</returns>
        public async Task<bool> VerifyAssetCreationStatus(string success)
        {
            try
            {
                // Construct xpath using AssetTagId and success parameter
                var successAlertXpath = string.Format(SuccessAlertXpathTemplate, AssetTagId);
                
                // Wait for and verify the success message is visible
                await _page.WaitForSelectorAsync(successAlertXpath, new PageWaitForSelectorOptions 
                { 
                    State = WaitForSelectorState.Visible, 
                    Timeout = 60000 
                });
                
                var successAlert = _page.Locator(successAlertXpath);
                return await successAlert.CountAsync() > 0;
            }
            catch (TimeoutException ex)
            {
                Console.Error.WriteLine($"Timeout error in VerifyAssetCreationStatus: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in VerifyAssetCreationStatus: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Searches for the created asset in the assets list using the AssetTagId.
        /// Navigates to the asset details page when found.
        /// </summary>
        /// <returns>True if asset was found and navigation was successful, false otherwise</returns>
        public async Task<bool> searchAssetFromListAndNavigateToDetails()
        {
            try
            {
                // Click on the search input field
                var searchInput = _page.Locator(SearchInputSelector);
                if (await searchInput.CountAsync() > 0)
                {
                    await searchInput.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    
                    // Type the AssetTagId into the search field
                    await searchInput.First.FillAsync(AssetTagId);
                    await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                }

                // Construct xpath for the search result mark element
                var searchResultMarkXpath = string.Format(SearchResultMarkTemplate, AssetTagId);
                
                // Wait for and verify the mark element is visible
                await _page.WaitForSelectorAsync(searchResultMarkXpath, new PageWaitForSelectorOptions 
                { 
                    State = WaitForSelectorState.Visible, 
                    Timeout = 60000 
                });
                
                var searchResultMark = _page.Locator(searchResultMarkXpath);
                if (await searchResultMark.CountAsync() > 0)
                {
                    // Click on the search result to navigate to asset details
                    await searchResultMark.First.ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    return true;
                }
                
                return false;
            }
            catch (TimeoutException ex)
            {
                Console.Error.WriteLine($"Timeout error in searchAssetFromListAndNavigateToDetails: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in searchAssetFromListAndNavigateToDetails: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Verifies the asset details on the asset page.
        /// Checks that asset tag, status, category (Laptops), and model match expected values.
        /// </summary>
        /// <param name="model">Expected model name</param>
        /// <param name="status">Expected status value</param>
        /// <returns>True if all asset details are verified successfully, false otherwise</returns>
        public async Task<bool> verifyAssetInfo(string model, string status)
        {
            try
            {
                var waitOptions = new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 60000 };
                
                // Verify Asset Tag
                var assetTagXpath = string.Format(AssetTagSpanTemplate, AssetTagId);
                await _page.WaitForSelectorAsync(assetTagXpath, waitOptions);
                Console.WriteLine($"Asset Tag verified: {AssetTagId}");

                // Verify Status
                var statusXpath = string.Format(StatusSelectorTemplate, status);
                await _page.WaitForSelectorAsync(statusXpath, waitOptions);
                Console.WriteLine($"Status '{status}' verified");

                // Verify Category Laptops
                await _page.WaitForSelectorAsync(CategoryLaptopsSelector, waitOptions);
                Console.WriteLine("Category 'Laptops' verified");

                // Verify Model
                var modelXpath = string.Format(ModelSelectorTemplate, model);
                await _page.WaitForSelectorAsync(modelXpath, waitOptions);
                Console.WriteLine($"Model '{model}' verified");

                return true;
            }
            catch (TimeoutException ex)
            {
                Console.Error.WriteLine($"Timeout error in verifyAssetInfo: Element not found - {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in verifyAssetInfo: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Navigates to the History tab on the asset details page.
        /// </summary>
        /// <returns>True if navigation to history tab was successful, false otherwise</returns>
        public async Task<bool> navigateToHistoryAndVerifyDetails()
        {
            try
            {
                await _page.Locator(HistoryTabSelector).ClickAsync(new LocatorClickOptions { Timeout = 60000 });
                Console.WriteLine("Successfully navigated to history tab");
                return true;
            }
            catch (TimeoutException ex)
            {
                Console.Error.WriteLine($"Timeout error in navigateToHistoryAndVerifyDetails: History tab not found - {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in navigateToHistoryAndVerifyDetails: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Verifies the asset history details in the History tab.
        /// Checks for 'create new' entry and validates the asset link with ID and model.
        /// </summary>
        /// <param name="model">Expected model name to verify in history</param>
        /// <returns>True if history details are verified successfully, false otherwise</returns>
        public async Task<bool> verifyAssetHistory(string model)
        {
            try
            {
                var waitOptions = new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 60000 };
                
                // Verify 'create new' text in history
                await _page.WaitForSelectorAsync(CreateNewHistorySelector, waitOptions);
                Console.WriteLine("History 'create new' verified");

                // Verify asset link with ID and model
                var assetHistoryLinkXpath = string.Format(AssetHistoryLinkTemplate, AssetTagId, model);
                await _page.WaitForSelectorAsync(assetHistoryLinkXpath, waitOptions);
                Console.WriteLine($"Asset history link verified: #{AssetTagId} - {model}");

                return true;
            }
            catch (TimeoutException ex)
            {
                Console.Error.WriteLine($"Timeout error in verifyAssetHistory: Expected element not found - {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in verifyAssetHistory: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

    }
}
