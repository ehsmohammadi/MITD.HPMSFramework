using MITD.DataAccess.EF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Data;
using System.Linq.Expressions;
using MITD.Domain.Repository;
using System.Collections.Generic;
using MITD.TestLayer.DataAccess.EF;
using MITD.TestLayer.DataAccess;
using MITD.TestLayer.Model;

namespace MITD.DataAcce.Test
{


    /// <summary>
    ///This is a test class for EFRepositoryTest and is intended
    ///to contain all EFRepositoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EFRepositoryTest
    {


        protected TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public virtual void MyTestInitialize()
        {
        }

        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        protected virtual IUnitOfWork CreateUnitOfWork()
        {
            return new EFUnitOfWork(new FrameWorkDBEntities());
        }

        protected virtual IRepository<T> CreateRepository<T>(IUnitOfWork unitOfWork) where T : class
        {
            return new EFRepository<T>(unitOfWork as EFUnitOfWork);
        }

        [TestMethod()]
        public void AddNewEntityTest()
        {
            #region Arrange

            var str = Guid.NewGuid().ToString();

            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                var entity = new TestEntity { TestProperty = str };
            #endregion

            #region Action
                target.Add(entity);

                unitofwork.Commit();
            }
                #endregion

            #region Assert
            using (var unitofwork = CreateUnitOfWork())
            {
                var rep = CreateRepository<TestEntity>(unitofwork);
                Assert.IsNotNull(rep.First(te => te.TestProperty == str));
            }
            #endregion

        }

        [TestMethod()]
        public void UpdateShouldAttachEntityWhenUOWIsNewTest()
        {
            #region Arrange

            var str = Guid.NewGuid().ToString();
            TestEntity entity = null;

            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                entity = new TestEntity { TestProperty = str };

                target.Add(entity);

                unitofwork.Commit();
            }

            #endregion

            #region Action
            str = Guid.NewGuid().ToString();
            entity.TestProperty = str;

            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork); 
                target.Update(entity);

                unitofwork.Commit();
            }
            #endregion

            #region Assert
            using (var unitofwork = CreateUnitOfWork())
            {
                var rep = CreateRepository<TestEntity>(unitofwork);
                Assert.IsNotNull(rep.First(te => te.Id == entity.Id && te.TestProperty == str));
            }
            #endregion

        }

        [TestMethod()]
        public void UpdatShouldChangeTheStateOfTheEntityWhenTheEntityIsAttachedTest()
        {
            #region Arrange

            var str = Guid.NewGuid().ToString();
            TestEntity entity = null;

            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                entity = new TestEntity { TestProperty = str };

                target.Add(entity);

                unitofwork.Commit();
            }

            #endregion

            #region Action
            str = Guid.NewGuid().ToString();
            entity.TestProperty = str;

            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork); 
                target.Attach(entity);
                target.Update(entity);

                unitofwork.Commit();
            }
            #endregion

            #region Assert
            using (var unitofwork = CreateUnitOfWork())
            {
                var rep = CreateRepository<TestEntity>(unitofwork);
                Assert.IsNotNull(rep.First(te => te.Id == entity.Id && te.TestProperty == str));
            }
            #endregion

        }

        [TestMethod()]
        public void UpdateShouldApplyCurrentValueOfTheEntityToTheEntityAttachedWithTheSameKey()
        {
            #region Arrange

            var str = Guid.NewGuid().ToString();
            TestEntity entity = null;

            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                entity = new TestEntity { TestProperty = str };

                target.Add(entity);

                unitofwork.Commit();
            }

            #endregion

            #region Action
            str = Guid.NewGuid().ToString();
            entity.TestProperty = str;

            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                target.First(te => te.Id == entity.Id);
                target.Update(entity);

                unitofwork.Commit();
            }
            #endregion

            #region Assert
            using (var unitofwork = CreateUnitOfWork())
            {
                var rep = CreateRepository<TestEntity>(unitofwork);
                Assert.IsNotNull(rep.First(te => te.Id == entity.Id && te.TestProperty == str));
            }
            #endregion

        }

        [TestMethod()]
        public void FindWithSortFetchStrategyShouldSortTheResult()
        {
            #region Arrange
            var lst = new List<string>();
            lst.Add(Guid.NewGuid().ToString());
            lst.Add(Guid.NewGuid().ToString());
            lst.Add(Guid.NewGuid().ToString());
            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                lst.ForEach(i =>
                {
                    var entity = new TestEntity { TestProperty = i };
                    target.Add(entity);
                });

                unitofwork.Commit();
            }

            #endregion

            #region Action

            var fs = new ListFetchStrategy<TestEntity>().OrderBy(t => t.TestProperty);
            IList<TestEntity> res;
            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                res = target.Find(t => lst.Contains(t.TestProperty), fs);
            }

            #endregion

            #region Assert
            var orderedLst = lst.OrderBy(i => i).ToList();
            Assert.AreEqual(orderedLst[0], res[0].TestProperty);
            Assert.AreEqual(orderedLst[1], res[1].TestProperty);
            Assert.AreEqual(orderedLst[2], res[2].TestProperty);
            #endregion
        }

        [TestMethod()]
        public void FirstWithIncludeInFetchStrategyShouldEagerLoadData()
        {
            #region Arrange

            TestEntity ent;
            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                ent = new TestEntity { TestProperty = "1" };
                ent.EntityItems.Add(new TestEntityItem { TestProperty = "2" , TestEntity=ent});
                ent.EntityItems.Add(new TestEntityItem { TestProperty = "3", TestEntity = ent });
                target.Add(ent);
                unitofwork.Commit();
            }

            #endregion

            #region Action

            var fs = new SingleResultFetchStrategy<TestEntity>().Include(t => t.EntityItems);
            TestEntity res;
            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                res = target.First(t => t.Id == ent.Id, fs);
                unitofwork.Commit();
            }
            
            #endregion

            #region Assert
            Assert.IsTrue(res.EntityItems.Count() == 2);
            #endregion

        }

        [TestMethod()]
        public void FindWithPagingInFetchStrategyShouldLoadOnePage()
        {
            #region Arrange

            var lst = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                lst.Add(Guid.NewGuid().ToString());
            }
            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                lst.ForEach(i =>
                {
                    var entity = new TestEntity { TestProperty = i };
                    target.Add(entity);
                });

                unitofwork.Commit();
            }
            #endregion

            #region Action

            var fs = new ListFetchStrategy<TestEntity>().OrderBy(t => t.TestProperty).WithPaging(2, 2);
            IList<TestEntity> res;
            using (var unitofwork = CreateUnitOfWork())
            {
                var target = CreateRepository<TestEntity>(unitofwork);
                res = target.Find(t => lst.Contains(t.TestProperty), fs);
            }

            #endregion

            #region Assert
            var orderedLst = lst.OrderBy(i => i).ToList();
            Assert.AreEqual(orderedLst[2], res[0].TestProperty);
            Assert.AreEqual(orderedLst[3], res[1].TestProperty);
            #endregion

        }

    }
}
