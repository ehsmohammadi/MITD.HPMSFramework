namespace MITD.TestLayer.DataAccess.EF
{
    #pragma warning disable 1573
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Infrastructure;
    using MITD.TestLayer.Model;
    using System.ComponentModel.DataAnnotations.Schema;
    
    internal partial class TestEntityItem_Mapping : EntityTypeConfiguration<TestEntityItem>
    {
        public TestEntityItem_Mapping()
        {
            this.HasKey(t => new { t.Id, t.TestEntityId });
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
    		this.ToTable("TestEntityItems");
    		this.Property(t => t.TestProperty).IsRequired().HasMaxLength(100);
            this.HasRequired(t => t.TestEntity).WithMany(t => t.EntityItems).HasForeignKey(d => d.TestEntityId);
    	}
    }
}
