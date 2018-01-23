using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Echeers.Mq.Data;

namespace Echeers.Mq.Services
{
    public class DataCleaner
    {
        public MqDbContext _dbContext;
        public DataCleaner(MqDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task StartCleanerService()
        {
            await TimeoutCleaner.AllClean(_dbContext);
        }
    }
}