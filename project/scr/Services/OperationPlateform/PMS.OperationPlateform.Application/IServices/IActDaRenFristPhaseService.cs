using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    using Domain.Entitys;

    public interface IActDaRenFristPhaseService
    {
        ActDaRenFristPhase GetTheLast();
    }
}