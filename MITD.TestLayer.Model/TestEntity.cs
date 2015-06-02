namespace MITD.TestLayer.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class TestEntity
    {
        public TestEntity()
        {
            this.EntityItems = new List<TestEntityItem>();
        }
    
        public virtual int Id { get; set; }
        public virtual string TestProperty { get; set; }
    
        public virtual IList<TestEntityItem> EntityItems { get; set; }
    }
}
