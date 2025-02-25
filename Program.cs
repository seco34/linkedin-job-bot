using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

class LinkedInJobBot
{
    static void Main()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--start-maximized"); // Tam ekran aç
        options.AddArgument("--disable-notifications"); // Bildirimleri kapat
        options.AddArgument("--disable-gpu"); // GPU hatasını önlemek için

        IWebDriver driver = new ChromeDriver(options);
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        try
        {
            // 1️⃣ LinkedIn giriş sayfasına git
            driver.Navigate().GoToUrl("https://www.linkedin.com/login");

            wait.Until(d => d.FindElement(By.Id("username"))).SendKeys("youmail@gmail.com");
            driver.FindElement(By.Id("password")).SendKeys("youpassword");
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            Thread.Sleep(5000); // Giriş sonrası bekleme

            // 2️⃣ İş ilanları sayfasına git
            driver.Navigate().GoToUrl("https://www.linkedin.com/jobs/");
            Thread.Sleep(5000);

            // 3️⃣ Sayfadaki tüm iş ilanlarını bul
            var jobListings = driver.FindElements(By.XPath("//ul[contains(@class,'jobs-search__results-list')]/li"));

            Console.WriteLine($"🔎 Toplam {jobListings.Count} ilan bulundu.");

            if (jobListings.Count == 0)
            {
                Console.WriteLine("❌ Hiç iş ilanı bulunamadı. XPath'i kontrol edin.");
                return;
            }

            foreach (var job in jobListings)
            {
                try
                {
                    job.Click(); // İlanı aç
                    Thread.Sleep(3000);

                    // 4️⃣ "Kolay Başvuru" butonunu bul ve tıkla
                    IWebElement easyApplyButton;
                    try
                    {
                        easyApplyButton = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(),'Kolay Başvuru')]")));
                        easyApplyButton.Click();
                        Console.WriteLine("✅ 'Kolay Başvuru' butonuna tıklandı.");
                        Thread.Sleep(2000);
                    }
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("⚠️ Bu ilana Easy Apply ile başvurulamıyor. Sonraki ilana geçiliyor...");
                        continue;
                    }

                    // 5️⃣ Eğer "İleri" butonu (ember883) çıkarsa, tıkla
                    ClickIfExists(driver, wait, "//*[@id='ember883']/span", "✅ İlk 'İleri' butonuna tıklandı.");
                    Thread.Sleep(2000);

                    // 6️⃣ Eğer ikinci "İleri" butonu (ember883) çıkarsa, tıkla
                    ClickIfExists(driver, wait, "//*[@id='ember883']/span", "✅ İkinci 'İleri' butonuna tıklandı.");
                    Thread.Sleep(2000);

                    // 7️⃣ "Başvuruyu Gönder" butonuna tıkla
                    ClickIfExists(driver, wait, "//*[@id='ember1349']/span", "✅ Başvuru tamamlandı!");
                    Thread.Sleep(3000);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Hata oluştu: " + ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Genel hata oluştu: " + ex.Message);
        }
        finally
        {
            // Tarayıcıyı kapat
            Thread.Sleep(5000);
            driver.Quit();
        }
    }

    /// <summary>
    /// Belirtilen XPath ile bir butonu kontrol eder ve varsa tıklar
    /// </summary>
    private static void ClickIfExists(IWebDriver driver, WebDriverWait wait, string xpath, string successMessage)
    {
        try
        {
            IWebElement button = wait.Until(d => d.FindElement(By.XPath(xpath)));
            if (button.Displayed && button.Enabled)
            {
                button.Click();
                Console.WriteLine(successMessage);
            }
        }
        catch (NoSuchElementException)
        {
            Console.WriteLine($"⚠️ {xpath} butonu bulunamadı, devam ediliyor...");
        }
    }
}
