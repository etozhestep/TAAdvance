using OpenQA.Selenium;

namespace TAF.Business.WebElements;

public class Grid(IWebDriver driver, By locator) : UiElement(driver, locator)
{
    private readonly By _headerButtonList = By.XPath("//*[contains(@class,'headerCell__header-cell')]");

    private readonly By _headerListXpath = By.XPath("//div[contains(@class,'gridHeader')]");
    private readonly By _headerTitleXpath = By.XPath("//span[contains(@class,'headerCell__title-full')]");
    private readonly By _rowXpath = By.XPath("//div[@data-id]");

    public IEnumerable<IWebElement> GetAllHeaders()
    {
        return
            GetElementHeaderList().Select(element => element.FindElement(_headerTitleXpath));
    }

    public IEnumerable<Launch> GetAllLaunches()
    {
        var launchElements = Waits.WaitForElementsPresence(_rowXpath);
        return launchElements.Select(element => new Launch(Driver, element));
    }

    public string SortedByName()
    {
        // Get all header elements
        var headers = GetElementHeaderList();

        // Find the header that is currently sorted
        var sortedElement = headers.FirstOrDefault(element =>
        {
            var headerClass = element.GetAttribute("class");
            return headerClass != null && headerClass.Contains("sorting-active");
        });

        if (sortedElement == null) throw new Exception("No sorted column found.");

        // Get the column name
        return sortedElement.Text.ToLower();
    }

    public string SortedByDirection()
    {
        // Get all header elements
        var headers = GetElementHeaderList();

        // Find the header that is currently sorted
        var sortedElement = headers.FirstOrDefault(element =>
        {
            var headerClass = element.GetAttribute("class");
            return headerClass != null && headerClass.Contains("sorting-active");
        });

        if (sortedElement == null) throw new Exception("No sorted column found.");
        
        // Find the sort arrow element
        var sortDirectionClass = sortedElement.GetAttribute("class");

        string sortBy;
        if (sortDirectionClass.Contains("sorting-desc"))
            sortBy = "desc";
        else if (sortDirectionClass.Contains("sorting-asc"))
            sortBy = "asc";
        else
            sortBy = "unknown";

        return sortBy;
    }
    private IEnumerable<IWebElement> GetElementHeaderList()
    {
        return Waits.WaitForElementsPresence(_headerButtonList);
    }

    public IEnumerable<string> GetHeaderNames()
    {
        return Waits.WaitForElementsPresence(_headerButtonList)
            .Select(element => element.Text)
            .Where(text => !string.IsNullOrEmpty(text));
    }

    public void SortByColumn(string columnName, string sortDirection)
    {
        Logger.Info($"Attempting to sort by column '{columnName}' in '{sortDirection}' order.");

        var headers = GetElementHeaderList();

        var targetHeader = headers.FirstOrDefault(element =>
        {
            var headerText = element.Text;
            return headerText.Equals(columnName, StringComparison.OrdinalIgnoreCase);
        });

        if (targetHeader == null)
            throw new Exception($"Column with name '{columnName}' not found.");

        // Determine current sort state
        var headerClass = targetHeader.GetAttribute("class");
        var isSorted = headerClass.Contains("sorting-active");
        var isDescending = headerClass.Contains("sorting-desc");
        var isAscending = headerClass.Contains("sorting-asc");

        // Decide if we need to click to achieve the desired sort direction
        if (!isSorted ||
            (sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) && isDescending) ||
            (sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) && isAscending))
        {
            Logger.Info($"Clicking on column '{columnName}' to sort.");
            targetHeader.Click();

            // After clicking, refresh the class info
            headerClass = targetHeader.GetAttribute("class");
            isDescending = headerClass.Contains("sorting-desc");
            isAscending = headerClass.Contains("sorting-asc");

            // Verify if desired sort a direction achieved
            if ((!sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) || isAscending) &&
                (!sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) || isDescending)) return;
            Logger.Info($"Desired sort direction not achieved, clicking again on column '{columnName}'.");
            targetHeader.Click();
        }
        else
        {
            Logger.Info($"Column '{columnName}' is already sorted in '{sortDirection}' order.");
        }
    }
}