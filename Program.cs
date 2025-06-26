using System.Net.Http.Headers;
using System.Text.Json;
using DotNetEnv;

Env.Load();

try
{
    Console.WriteLine("Starting PlatformWellSync application...");
    
    var api = new Api();
    var db = new AppDbContext();

    var username = Environment.GetEnvironmentVariable("USERNAME") ?? "";
    var password = Environment.GetEnvironmentVariable("PASSWORD") ?? "";

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
        Console.WriteLine("Username or password is not set. Please set the USERNAME and PASSWORD environment variables.");
        return;
    }

    var result = await api.login(username, password);

    Console.WriteLine("Login successful");

    var platforms = await api.getPlatforms();

    foreach (var platform in platforms) {
        var updatedPlatform = db.createOrUpdatePlatform(platform);
        
        foreach (var well in updatedPlatform.Well) {
            var updatedWell = db.createOrUpdateWell(well);
        }
    }
    Console.WriteLine("Saving changes to database...");
    await db.SaveChangesAsync();

    Console.WriteLine("Platforms saved to database");
} catch (Exception ex) {
    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    if (ex.InnerException != null) {
        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
} finally {
    Console.WriteLine("Application finished.");
}





