using Shared;
using Shared.Models;

namespace ScheduleAPI.Services;

public class FilterService(Repository repository, IConfiguration configuration)
{
    /// <summary>
    /// Filters a list of lessons based on the given parameters.
    /// </summary>
    /// <param name="filterParams">
    /// A dictionary of filter parameters where the key represents the lesson type and the value represents the subgroup numbers
    /// </param>
    /// <returns>
    /// A list of lessons matching the provided filter criteria.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the "groupname" parameter is missing from the filter parameters.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the "SubgroupCodes" configuration section is missing or invalid.
    /// </exception>
    public async Task<List<Lesson>> FilterLessons(Dictionary<string, string> filterParams)
    {
        if (!filterParams.TryGetValue("groupname", out var group))
            throw new ArgumentException("groupname parameter not found in filter params");

        var lessons = await repository.GetGroupSchedule(group);
        var config = configuration.GetSection("SubgroupCodes").Get<Dictionary<string, string>>();

        if (config == null)
            throw new Exception("SubgroupCodes not found in configuration");

        var filtered = new List<Lesson>().AsEnumerable();

        foreach (var kvp in filterParams)
        {
            if (kvp.Key == "default")
            {
                filtered = filtered.Concat(
                    lessons.Where(x =>
                        !x.IsOptional && (x.Subgroup == null || kvp.Value.Contains(x.Subgroup.ToString()))));
            }
            else if (config.TryGetValue(kvp.Key, out var value))
            {
                filtered = filtered.Concat(
                    lessons.Where(x =>
                        x.Name.Contains(value) && (x.Subgroup == null || kvp.Value == "0" || kvp.Value.Contains(x.Subgroup.ToString()))));
            }
        }

        var res = filtered.ToList();
        return res;
    }
}