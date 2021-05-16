using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using LocatingApp.Entities;
using LocatingApp.Services.MPlaceChecking;
using LocatingApp.Services.MAppUser;
using LocatingApp.Services.MPlace;
using LocatingApp.Services.MCheckingStatus;

namespace LocatingApp.Rpc.place_checking
{
    public partial class PlaceCheckingController : RpcController
    {
    }
}

