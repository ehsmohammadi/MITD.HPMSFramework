namespace MITD.TestLayer.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class TestEntityItem
    {
        public virtual int Id { get; set; }
        public virtual int TestEntityId { get; set; }
        public virtual string TestProperty { get; set; }
    
        public virtual TestEntity TestEntity { get; set; }
    }
}
