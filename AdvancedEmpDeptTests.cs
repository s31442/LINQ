namespace Tutorial3Tests;

public class AdvancedEmpDeptTests
{
    [Fact]
    public void ShouldReturnMaxSalary()
    {
        var emps = Database.GetEmps();

        decimal? maxSalary = emps.Max(e => e.Sal);

        Assert.Equal(5000, maxSalary);
    }

    [Fact]
    public void ShouldReturnMinSalaryInDept30()
    {
        var emps = Database.GetEmps();

        decimal? minSalary = emps
            .Where(e => e.DeptNo == 30)
            .Min(e => e.Sal);

        Assert.Equal(1250, minSalary);
    }

    [Fact]
    public void ShouldReturnFirstTwoHiredEmployees()
    {
        var emps = Database.GetEmps();

        var firstTwo = emps
            .OrderBy(e => e.HireDate)
            .Take(2)
            .ToList();

        Assert.Equal(2, firstTwo.Count);
        Assert.True(firstTwo[0].HireDate <= firstTwo[1].HireDate);
    }

    [Fact]
    public void ShouldReturnDistinctJobTitles()
    {
        var emps = Database.GetEmps();

        var jobs = emps
            .Select(e => e.Job)
            .Distinct()
            .ToList();

        Assert.Contains("PRESIDENT", jobs);
        Assert.Contains("SALESMAN", jobs);
    }

    [Fact]
    public void ShouldReturnEmployeesWithManagers()
    {
        var emps = Database.GetEmps();

        var withMgr = emps
            .Where(e => e.Mgr != null)
            .ToList();

        Assert.All(withMgr, e => Assert.NotNull(e.Mgr));
    }

    [Fact]
    public void AllEmployeesShouldEarnMoreThan500()
    {
        var emps = Database.GetEmps();

        var result = emps.All(e => e.Sal > 500);

        Assert.True(result);
    }

    [Fact]
    public void ShouldFindAnyWithCommissionOver400()
    {
        var emps = Database.GetEmps();

        var result = emps.Any(e => e.Comm.HasValue && e.Comm > 400);

        Assert.True(result);
    }

    [Fact]
    public void ShouldReturnEmployeeManagerPairs()
    {
        var emps = Database.GetEmps();

        var result = emps
            .Join(emps,
                e1 => e1.Mgr,
                e2 => e2.EmpNo,
                (e1, e2) => new { Employee = e1.EName, Manager = e2.EName })
            .ToList();

        Assert.Contains(result, r => r.Employee == "SMITH" && r.Manager == "FORD");
    }

    [Fact]
    public void ShouldReturnTotalIncomeIncludingCommission()
    {
        var emps = Database.GetEmps();

        var result = emps
            .Select(e => new
            {
                e.EName,
                Total = e.Sal + (e.Comm ?? 0)
            })
            .ToList();

        Assert.Contains(result, r => r.EName == "ALLEN" && r.Total == 1900);
    }

    [Fact]
    public void ShouldJoinEmpDeptSalgrade()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();
        var grades = Database.GetSalgrades();

        var result = emps
            .Join(depts,
                e => e.DeptNo,
                d => d.DeptNo,
                (e, d) => new { e, d })
            .SelectMany(
                ed => grades
                    .Where(s => ed.e.Sal >= s.Losal && ed.e.Sal <= s.Hisal)
                    .Select(s => new { ed.e.EName, ed.d.DName, s.Grade }))
            .ToList();

        Assert.Contains(result, r => r.EName == "ALLEN" && r.DName == "SALES" && r.Grade == 3);
    }
}
