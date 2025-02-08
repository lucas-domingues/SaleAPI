using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Tests.TestHelpers
{
    public class FakeEntityEntry<TEntity> : EntityEntry<TEntity> where TEntity : class
    {
        public FakeEntityEntry(TEntity entity) : base(null)
        {
            Entity = entity;
        }

        public override TEntity Entity { get; }

        private EntityState _state;
        public override EntityState State
        {
            get => _state;
            set => _state = value;
        }
    }
}
