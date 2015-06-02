using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders;

namespace MITD.Core.RuleEngine.NH
{
	[Migration(1)]
	public class RuleEngineMigrationVer001 : Migration
	{
		public override void Up()
		{
			Create.Table("NH_Hilo")
				.WithColumn("TableKey").AsString(100).NotNullable()
				.WithColumn("NextHi").AsInt64().NotNullable();
			Create.Table("REConfigItems")
				.WithColumn("DbId").AsInt64().PrimaryKey()
				.WithColumn("Name").AsString(100).Unique("idx_Name_Unique").NotNullable()
				.WithColumn("Value").AsString(1024);
			Create.Table("Rules")
				.WithColumn("Id").AsInt64().PrimaryKey()
				.WithColumn("RuleText").AsString(int.MaxValue)
				.WithColumn("RuleType").AsInt32().NotNullable();
			Create.Table("RuleFunctions")
				.WithColumn("Id").AsInt64().PrimaryKey()
				.WithColumn("FunctionsText").AsString(int.MaxValue);
			Create.Table("Periods")
				.WithColumn("Id").AsInt64().PrimaryKey();
			Create.Table("Employees")
				.WithColumn("Id").AsInt64().PrimaryKey();
			Create.Table("Policies")
				.WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("RuleFunctionId").AsInt64().Nullable().ForeignKey("fk_Policies_RuleFunctions_RuleFunctionId", "RuleFunctions", "Id");
			Create.Table("Policies_RE")
				.WithColumn("Id").AsInt64().PrimaryKey().ForeignKey("fk_Policies_PoliciesRE_Id", "Policies", "Id");
			Create.Table("Policies_RE_Rules")
				.WithColumn("Id").AsInt64().PrimaryKey().Identity()
				.WithColumn("PolicyId").AsInt64().NotNullable().ForeignKey("fk_Policies_RE_Rules_Policies_RE_Id", "Policies_RE", "Id")
				.WithColumn("RuleId").AsInt64().NotNullable().ForeignKey("fk_Policies_RE_Rules_Rules_Id", "Rules", "Id");
			Create.Table("Calculations")
				.WithColumn("Id").AsInt64().PrimaryKey()
				.WithColumn("Name").AsString(100).NotNullable().Unique("idx_Name_Unique")
				.WithColumn("PolicyId").AsInt64().NotNullable().ForeignKey("fk_Policies_Calculations_PolicyId", "Policies", "Id")
				.WithColumn("PeriodId").AsInt64().NotNullable().ForeignKey("fk_Periods_Calculations_PeriodId", "Periods", "Id")
				.WithColumn("StartRunTime").AsDateTime().Nullable()
				.WithColumn("EndRunTime").AsDateTime().Nullable()
				.WithColumn("employeeIdDelimetedList").AsString(int.MaxValue).NotNullable();


			Execute.Sql(@"
CREATE SEQUENCE [dbo].[PeriodSeq] 
 AS [bigint]
 START WITH 1
 INCREMENT BY 1
 MINVALUE -9223372036854775808
 MAXVALUE 9223372036854775807
 CACHE ");

			Execute.Sql(@"
CREATE SEQUENCE [dbo].[PolicySeq] 
 AS [bigint]
 START WITH 1
 INCREMENT BY 1
 MINVALUE -9223372036854775808
 MAXVALUE 9223372036854775807
 CACHE ");

			Execute.Sql(@"
CREATE SEQUENCE [dbo].[CalculationSeq] 
 AS [bigint]
 START WITH 1
 INCREMENT BY 1
 MINVALUE -9223372036854775808
 MAXVALUE 9223372036854775807
 CACHE ");
			Execute.Sql(@"
CREATE SEQUENCE [dbo].[EmployeeSeq] 
 AS [bigint]
 START WITH 1
 INCREMENT BY 1
 MINVALUE -9223372036854775808
 MAXVALUE 9223372036854775807
 CACHE ");
			Execute.Sql(@"
CREATE SEQUENCE [dbo].[RuleSeq] 
 AS [bigint]
 START WITH 1
 INCREMENT BY 1
 MINVALUE -9223372036854775808
 MAXVALUE 9223372036854775807
 CACHE ");
			Execute.Sql(@"
CREATE SEQUENCE [dbo].[RuleFunctionSeq] 
 AS [bigint]
 START WITH 1
 INCREMENT BY 1
 MINVALUE -9223372036854775808
 MAXVALUE 9223372036854775807
 CACHE ");
			Execute.Sql(@"
CREATE SEQUENCE [dbo].[REConfigItemSeq] 
 AS [bigint]
 START WITH 3
 INCREMENT BY 1
 MINVALUE -9223372036854775808
 MAXVALUE 9223372036854775807
 CACHE ");
			Insert.IntoTable("NH_Hilo").Row(new
				{
					TableKey = "REConfigItems",
					NextHi = 3
				});
			Insert.IntoTable("REConfigItems").Row(new
			{
				DbId = 1,
				Name = "RuleTextTemplate",
				Value =
@"
	public class <#classname#> : IRule<CalculationData>
	{
		public void Execute(CalculationData data)
		{
			<#ruletext#>
		}
	}"
            }).Row(new
			{
				DbId = 2,
				Name = "ReferencedAssemblies",
				Value =
@"System.Core.dll;MITD.Core.RuleEngine.dll;MITD.PMS.Domain.dll"
			}).Row(new
			{
				DbId = 3,
				Name = "LibraryTextTemplate",
				Value =
@"
	using System;
	using System.Collections.Generic;
	using MITD.Core;
	using MITD.Core.RuleEngine;
    using MITD.PMS.Domain.Model.JobIndexPoints;
    using MITD.PMS.Domain.Model;
	using System.Linq;
	using System.Globalization;

	namespace MITD.Core.RuleEngine
	{
		using MITD.Core.RuleEngine;

		public static class RuleExecutionUtil
		{
			public static List<JobIndexPoint> Res =  new List<JobIndexPoint>();
			<#functions#>
		}
		
		public class RuleResult : IRuleResult<List<JobIndexPoint>>
		{
			public List<JobIndexPoint> GetResult()
			{
				return RuleExecutionUtil.Res;
			}
		}

		<#rules#>
	}"
            });
		}

		public override void Down()
		{
            Delete.Table("Calculations");
            Delete.Table("Policies_RE_Rules");
            Delete.Table("Policies_RE");
            Delete.Table("Policies");
            Delete.Table("Rules");
			Delete.Table("RuleFunctions");
			Delete.Table("REConfigItems");
			Delete.Table("NH_Hilo");
			Delete.Table("Employees");
			Delete.Table("Periods");
			Execute.Sql("Drop sequence [dbo].[RuleSeq]");
			Execute.Sql("Drop sequence [dbo].[RuleFunctionSeq]");
			Execute.Sql("Drop sequence [dbo].[REConfigItemSeq]");
			Execute.Sql("Drop sequence [dbo].[EmployeeSeq]");
			Execute.Sql("Drop sequence [dbo].[PolicySeq]");
			Execute.Sql("Drop sequence [dbo].[PeriodSeq]");
			Execute.Sql("Drop sequence [dbo].[CalculationSeq]");
		}
	}
}
