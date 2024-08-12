using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result.Org
{
    public  class CardListResult<TCard> : BaseResult<IEnumerable<TCard>> where TCard:class
    {
    }
}
