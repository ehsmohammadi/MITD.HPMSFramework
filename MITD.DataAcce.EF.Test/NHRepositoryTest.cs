using MITD.DataAccess.EF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Data;
using System.Linq.Expressions;
using MITD.Domain.Repository;
using System.Collections.Generic;
using MITD.TestLayer.Model;
using MITD.Data.NH;
using SIC.WorkflowApp.Data.NH;
using NHibernate.Linq;
using NHibernate;
using NHibernate.Transform;
using NHibernate.Criterion;

namespace MITD.DataAcce.Test
{
    /// <summary>
    ///This is a test class for EFRepositoryTest and is intended
    ///to contain all EFRepositoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NHRepositoryTest : EFRepositoryTest
    {
        EntityFactory entityFactory;
        [TestInitialize]
        public override void MyTestInitialize()
        {
            var seqSrv = new SequenceService();
            seqSrv.RegisterProvider(new EntitySequenceProvider((CreateUnitOfWork() as NHUnitOfWork).Session.SessionFactory));
            entityFactory = new EntityFactory(seqSrv);

            base.MyTestInitialize();
        }

        protected override IUnitOfWork CreateUnitOfWork()
        {
            return new NHUnitOfWork(FrameworkDBSession.GetSession());
        }

        protected override IRepository<T> CreateRepository<T>(IUnitOfWork unitofwork)
        {
            return new NHRepository<T>(unitofwork as NHUnitOfWork);
        }

        [TestMethod()]
        public void AddNewEntityWithCompositeIdentifireTest()
        {
            #region Arrange

            var str = Guid.NewGuid().ToString();

            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<Entity>(unitofwork);
                var entity = entityFactory.Create(str);
            #endregion

            #region Action
                target.Add(entity);

                unitofwork.Commit();
            }
                #endregion

            #region Assert
            using (var unitofwork = CreateUnitOfWork())
            {
                var rep = CreateRepository<Entity>(unitofwork);
                Assert.IsNotNull(rep.First(te => te.TestProperty == str));
            }
            #endregion

        }

        [TestMethod()]
        public void AddNewEntityItemWithCompositeIdentifireTest()
        {
            #region Arrange

            var str = Guid.NewGuid().ToString();
            var entity = entityFactory.Create(str);
            entity.AddEntityItem2(Guid.NewGuid().ToString());
            using (var unitofwork = CreateUnitOfWork())
            {
                var helper = CreateRepository<Entity>(unitofwork);
                helper.Add(entity);
                unitofwork.Commit();
            }

            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<EntityItem>(unitofwork);
                var entityItem = new EntityItem(entity.Id, str);
            #endregion

            #region Action
                target.Add(entityItem);

                unitofwork.Commit();
            }
                #endregion

            #region Assert
            using (var unitofwork = CreateUnitOfWork())
            {
                var rep = CreateRepository<Entity>(unitofwork);
                var en = rep.First(te => te.TestProperty == str,
                    new SingleResultFetchStrategy<Entity>().Include(e => e.EntityItems2));
            }
            #endregion

        }
        [TestMethod()]
        public void AddNewEntity2WithEntityIdListTest()
        {
            #region Arrange

            var lst = new List<Entity>();

            for (int i = 0; i < 2; i++)
            {
                var entity = entityFactory.Create(Guid.NewGuid().ToString());
                using (var unitofwork= CreateUnitOfWork())
                {
                    var helper = CreateRepository<Entity>(unitofwork);
                    helper.Add(entity);
                    unitofwork.Commit();
                    lst.Add(entity);
                }
            }

            var lst3 = new List<EntityId3>();

            for (int i = 0; i < 2; i++)
            {
                var entity3 = new Entity3(Guid.NewGuid().ToString(),"100");
                using (var unitofwork = CreateUnitOfWork())
                {
                    var helper = CreateRepository<Entity3>(unitofwork);
                    helper.Add(entity3);
                    unitofwork.Commit();
                    lst3.Add(entity3.Id);
                }
            }

            Entity2 entity2;
            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<Entity2>(unitofwork);
                entity2 = new Entity2("500");
                lst.ForEach(i => entity2.AddEntity(i));

                entity2.EntityIds3 = lst3;
            #endregion

                #region Action
                target.Add(entity2);

                unitofwork.Commit();
            }
                #endregion

            #region Assert
            Entity2 en;
            using (var unitofwork = CreateUnitOfWork())
            {
                var rep = CreateRepository<Entity2>(unitofwork);
                en = rep.Single(te => te.Id == entity2.Id);
            }
            Assert.IsTrue(en.EntityIds.Count() == 2);
            #endregion

        }

        interface ISequenceProvider
        {
        }
        interface ISequenceProvider<T> : ISequenceProvider
        {
            long GetNext();
        }

        class EntitySequenceProvider : ISequenceProvider<Entity>
        {
            private ISessionFactory sf;
            public EntitySequenceProvider(ISessionFactory sf)
            {
                this.sf = sf;
            }
            public long GetNext()
            {
                long res = 0;
                using (var session = sf.OpenStatelessSession())
                {
                    session.BeginTransaction();
                    res = session.CreateSQLQuery("select next value for [dbo].[EntitySeq]").UniqueResult<long>();
                    session.Transaction.Commit();
                }
                return res;
            }
        }

      
        interface ISequenceService
        {
            long GetNext<T>();
        }

        class SequenceService : ISequenceService
        {
            List<ISequenceProvider> lst = new List<ISequenceProvider>();

            public long GetNext<T>()
            {
                return lst.OfType<ISequenceProvider<T>>().Single().GetNext();
            }
            public void RegisterProvider<T>(ISequenceProvider<T> provider)
            {
                lst.Add(provider);
            }
        }
        
        class EntityFactory
        {
            private  ISequenceService sqSrv;
            public EntityFactory(ISequenceService sqSrv)
            {
                this.sqSrv = sqSrv;
            }
            public Entity Create(string testProperty)
            {
                return new Entity(new EntityId(sqSrv.GetNext<Entity>()), testProperty);
            }
        }
        
        [TestMethod()]
        public void TestMethod1()
        {
            Entity2 e;
            Entity e2;
            List<Entity> lst;
            using (var uow = CreateUnitOfWork())
            {
                var rep2 = CreateRepository<Entity>(uow);
                lst = new List<Entity>();
                e2 = entityFactory.Create(Guid.NewGuid().ToString());
                rep2.Add(e2);
                lst.Add(e2);
                e2 = entityFactory.Create(Guid.NewGuid().ToString());
                rep2.Add(e2);
                lst.Add(e2);
                
                e = new Entity2(Guid.NewGuid().ToString());
                lst.ForEach(i => e.AddEntity(i));
                var rep = CreateRepository<Entity2>(uow);
                rep.Add(e);

                e2 = entityFactory.Create(Guid.NewGuid().ToString());
                rep2.Add(e2);
                
                uow.Commit();
            }

            e.RemoveEntity(lst[0]);
            e.AddEntity(e2);

            using (var uow = CreateUnitOfWork())
            {
                var rep = CreateRepository<Entity2>(uow);
                rep.Update(e);
                uow.Commit();
            }

            //using (var uow = CreateUnitOfWork())
            //{
            //    var rep = CreateRepository<Entity2>(uow);
            //    var ee = rep.Single(f => f.Id == e.Id);
            //    ee.RemoveEntity(lst[0]);
            //    ee.AddEntity(e2);

            //    uow.Commit();
            //}


        }
    }
}
