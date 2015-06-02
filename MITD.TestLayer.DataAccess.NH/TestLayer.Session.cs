using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Reflection;
using MITD.TestLayer.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Cfg;
using System.Data;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Driver;
using NHibernate.Dialect;
using MITD.Core;

namespace SIC.WorkflowApp.Data.NH
{
    public static class FrameworkDBSession
    {
        private static Lazy<ISessionFactory> sessionFactory =
            new Lazy<ISessionFactory>(() => createSessionFactory());

        private static ISessionFactory createSessionFactory()
        {
            var configure = new Configuration();
            configure.SessionFactoryName("FrameWorkDBEntities");

            configure.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2008Dialect>();
                db.Driver<SqlClientDriver>();
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = IsolationLevel.ReadCommitted;

                db.ConnectionStringName = "FrameWorkDBEntities";
                db.Timeout = 10;
            });

            configure.AddXmlFile("Entity2.hbm.xml");
            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly()
                .GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            configure.AddDeserializedMapping(mapping, "FrameWorkDBEntities");
            SchemaMetadataUpdater.QuoteTableAndColumns(configure);

            return configure.BuildSessionFactory();

        }

        public static ISession GetSession()
        {
            return sessionFactory.Value.OpenSession();
        }

        public static IStatelessSession GetStatelessSession()
        {
            return sessionFactory.Value.OpenStatelessSession();
        }
    }

    public class TestEntityMap : ClassMapping<TestEntity>
    {
        public TestEntityMap()
        {
            Table("TestEntities");
            Id(u => u.Id, u => u.Generator(Generators.Identity));
            Property(u => u.TestProperty, u =>
                {
                    u.Length(100);
                    u.NotNullable(true);
                });
            Bag(u => u.EntityItems, u =>
                {
                    u.Key(k =>
                        {
                            k.Column("TestEntityId");
                        });
                    u.Cascade(Cascade.All);
                    u.Inverse(true);
                }, u => u.OneToMany());
        }
    }

    public class TestEntityItemMap : ClassMapping<TestEntityItem>
    {
        public TestEntityItemMap()
        {
            Table("TestEntityItems");
            Id(u => u.Id, u =>
                {
                    u.Generator(Generators.Identity);
                });
            Property(u => u.TestProperty, u =>
            {
                u.Length(100);
                u.NotNullable(true);
            });
            ManyToOne(w => w.TestEntity, w =>
            {
                w.Column("TestEntityId");
                w.NotNullable(true);
            });
        }
    }

    public class EntityMap : ClassMapping<Entity>
    {
        public EntityMap()
        {
            Table("Entities");
            ComponentAsId(e => e.Id, e => e.Property(k => k.Id, k => k.Access(Accessor.Field)));
            Property(u => u.TestProperty, u =>
            {
                u.Length(100);
                u.NotNullable(true);
            });
            Bag(u => u.EntityItems2, u =>
            {
                u.Key(k => k.Column("EntityId"));
                u.Cascade(Cascade.All);
                u.Inverse(true);
            }, u => u.OneToMany());
        }
    }

    public class EntityItemMap : ClassMapping<EntityItem>
    {
        public EntityItemMap()
        {
            Table("EntityItems");
            Id("dbId", u =>
                {
                    u.Column("Id");
                    u.Generator(Generators.Identity);
                });
            Component(e => e.Id, e => e.Property(m => m.Id, m => m.Generated(PropertyGeneration.Always)));
            Property(u => u.TestProperty, u =>
            {
                u.Length(100);
                u.NotNullable(true);
            });
            Component(e => e.EntityId, e => e.Property(m => m.Id, m =>
                {
                    m.Column("EntityId");
                    m.Access(Accessor.Field);
                }));
        }
    }

    public class EntityItem2Map : ClassMapping<EntityItem2>
    {
        public EntityItem2Map()
        {
            Table("EntityItems2");
            Id("dbId", u =>
            {
                u.Column("Id");
                u.Generator(Generators.Identity);
            });
            Component(e => e.Id, e => e.Property(m => m.Id, m => m.Generated(PropertyGeneration.Always)));
            Property(u => u.TestProperty, u =>
            {
                u.Length(100);
                u.NotNullable(true);
            });
            ManyToOne(w => w.Entity, w =>
            {
                w.Column("EntityId");
                w.NotNullable(true);
            });
        }
    }

    public class Entity3Map : ClassMapping<Entity3>
    {
        public Entity3Map()
        {
            Id("dbId", m =>
            {
                m.Column("DbId");
                m.Generator(Generators.Identity);
            });
            Table("Entities3");
            Component(e => e.Id, e =>
            {
                e.Property(m => m.Id, m =>
                    {
                        m.Length(50);
                        m.Access(Accessor.Field);
                    });
                e.Property("dbId", m =>
                    {
                        m.Column("DbId");
                        m.Generated(PropertyGeneration.Always);
                    });
            });
            Property(u => u.TestProperty, u =>
            {
                u.Length(100);
                u.NotNullable(true);
            });
        }
    }

}

