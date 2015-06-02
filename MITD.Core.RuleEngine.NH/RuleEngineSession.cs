using MITD.Core.RuleEngine.Model;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Mapping.ByCode.Conformist;

namespace MITD.Core.RuleEngine.NH
{
    public static class RuleEngineSession
    {
        private static Lazy<ISessionFactory> sessionFactory =
            new Lazy<ISessionFactory>(() => createSessionFactory());
        public static string sessionName = "PMSDBConnection";

        private static ISessionFactory createSessionFactory()
        {
            var configure = new Configuration();
            configure.SessionFactoryName(sessionName);

            configure.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2008Dialect>();
                db.Driver<SqlClientDriver>();
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = System.Data.IsolationLevel.ReadCommitted;

                db.ConnectionStringName = sessionName;
                db.Timeout = 10;
            });

            //configure.AddXmlFile("Entity2.hbm.xml");
            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly()
                .GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            configure.AddDeserializedMapping(mapping, sessionName);
            SchemaMetadataUpdater.QuoteTableAndColumns(configure);

            return configure.BuildSessionFactory();
        }

        public static ISession GetSession()
        {
            return sessionFactory.Value.OpenSession();
        }

        public static ISession GetSession(System.Data.IDbConnection conn)
        {
            return sessionFactory.Value.OpenSession(conn);
        }


        public static IStatelessSession GetStatelessSession()
        {
            return sessionFactory.Value.OpenStatelessSession();
        }
    }

    public class RuleEngineConfigurationItemMap : ClassMapping<RuleEngineConfigurationItem>
    {
        public RuleEngineConfigurationItemMap()
        {
            Table("REConfigItems");
            Id("dbId", m =>
                {
                    m.Column("DbId");
                    m.Generator(Generators.HighLow, gm => gm.Params(new
                    {
                        table = "NH_HiLo",
                        column = "NextHi",
                        max_lo = 1,
                        where = String.Format("TableKey = '{0}'", "REConfigItems")
                    }));
                });
            Component(c => c.Id, c => 
            {
                c.Lazy(false);
                c.Access(Accessor.Field);
                c.Property(m => m.Name, m =>
                {
                    m.Length(100);
                    m.Access(Accessor.Field);
                });
                c.Property("dbId", m =>
                {
                    m.Column("DbId");
                    m.Generated(PropertyGeneration.Always);
                });
            });
            Property(c => c.Value, m =>
            {
                m.Length(1024);
                m.Access(Accessor.Field);
            });
        }
    }

    public class RuleBaseMap : ClassMapping<RuleBase>
    {
        public RuleBaseMap()
        {
            Table("RulesBase");
            ComponentAsId(c => c.Id, c =>
            {
                c.Access(Accessor.Field);
                c.Property(m => m.Id, m =>
                {
                    m.Access(Accessor.Field);
                });
            });
            Property(c => c.Name, m =>
            {
                m.Length(256);
                m.Access(Accessor.Field);
            });
            Property(c => c.RuleText, m =>
            {
                m.Length(int.MaxValue);
                m.Access(Accessor.Field);
            });
            Property(c => c.RuleType, m =>
            {
                m.Access(Accessor.Field);
                m.Type<MITD.Data.NH.EnumerationTypeConverter<RuleType>>();
            });
            Property(c => c.ExecuteOrder, m =>
            {
                m.Length(int.MaxValue);
                m.Access(Accessor.Field);
            });
        }
    }

    public class RuleMap :  JoinedSubclassMapping<Rule>
    {
        public RuleMap()
        {
            Table("Rules");
            Key(m => m.Column("Id"));
            Map<DateTime, RuleTrail>("rulesTrail", m =>
            {
                m.Key(k => k.Column("RuleId"));
                m.Cascade(Cascade.All);
                m.Inverse(true);
            }, m => m.Element(e => e.Column("EffectiveDate")),
                m => m.OneToMany());
        }
    }

    public class RuleTrailMap : JoinedSubclassMapping<RuleTrail>
    {
        public RuleTrailMap()
        {
            Table("RulesTrail");
            Key(r => r.Column("Id"));
            Property(r => r.EffectiveDate, m =>
            {
                m.Access(Accessor.Field);
            });
            ManyToOne(r => r.Current, m =>
            {
                m.Column("RuleId");
                m.Access(Accessor.Field);
            });
        }
    }

    public class RuleFunctionBaseMap : ClassMapping<RuleFunctionBase>
    {
        public RuleFunctionBaseMap()
        {
            Table("RuleFunctionsBase");
            ComponentAsId(c => c.Id, c =>
            {
                c.Access(Accessor.Field);
                c.Property(m => m.Id, m =>
                {
                    m.Access(Accessor.Field);
                });
            });
            Property(c => c.Name, m =>
            {
                m.Length(256);
                m.Access(Accessor.Field);
            });
            Property(c => c.FunctionsText, m =>
            {
                m.Length(int.MaxValue);
                m.Access(Accessor.Field);
            });
        }
    }

    public class RuleFunctionMap : JoinedSubclassMapping<RuleFunction>
    {
        public RuleFunctionMap()
        {
            Table("RuleFunctions");
            Key(r => r.Column("Id"));
            Map<DateTime, RuleFunctionTrail>("ruleFunctionsTrail", m =>
            {
                m.Key(k => k.Column("RuleFunctionId"));
                m.Cascade(Cascade.All);
                m.Inverse(true);
            }, m => m.Element(e => e.Column("EffectiveDate")),
                m => m.OneToMany());
        }
    }

    public class RuleFunctionTrailMap : JoinedSubclassMapping<RuleFunctionTrail>
    {
        public RuleFunctionTrailMap()
        {
            Table("RuleFunctionsTrail");
            Key(r => r.Column("Id"));
            Property(r => r.EffectiveDate, m =>
            {
                m.Access(Accessor.Field);
            });
            ManyToOne(r => r.Current, m =>
                {
                    m.Column("RuleFunctionId");
                    m.Access(Accessor.Field);
                });
        }
    }

}
