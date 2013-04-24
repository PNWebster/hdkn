using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.Data;
using HdknPlugins.AutoAdd.Data.Models;

namespace HdknPlugins.AutoAdd.Http.Api
{
    public class FoldersController : HttpApiController
    {
        private readonly IDataRepository _repository;

        public FoldersController(IDataRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            _repository = repository;
        }

        public IEnumerable<WatchedFolder> Get()
        {
            return _repository.List<WatchedFolder>();
        } 
    }
}
