using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Dialogs;

namespace Kassa.Tests;
[TestClass]
public class MethodOfDivisionTests
{
    [TestMethod]

    public void IntoSeveralEqualPartTest()
    {
        // We are not testing the ApplyCommand
        // which is why we pass null
        // We are only testing the logic of 
        // TotalServing = CountOfServing * ServingDivider
        var vm = new PortionDialogVm.IntoSeveralEqualPartsVm(null!, null!, null!)
        {
            TotalServing = 10,
            CountOfServing = 5
        };

        // 10 = 5 * ServingDivider
        // ServingDivider = 10 / 5 = 2

        Assert.AreEqual(2, vm.ServingDivider);
    }

    [TestMethod]
    public void IntoSeveralEqualPartChangingTotalTest()
    {
        // We are not testing the ApplyCommand
        // which is why we pass null
        // We are only testing the logic of 
        // TotalServing = CountOfServing * ServingDivider
        var vm = new PortionDialogVm.IntoSeveralEqualPartsVm(null!, null!, null!)
        {
            TotalServing = 10,
            CountOfServing = 5
        };

        // 10 = 5 * ServingDivider
        // ServingDivider = 10 / 5 = 2

        vm.TotalServing = 13.33;

        // 13.33 = ServingDivider * 5
        // ServingDivider = 13.33 / 5 = 2.666

        Assert.AreEqual(2.666, vm.ServingDivider);
    }

    [TestMethod]
    public void IntoSeveralEqualPartChangingCountOfServingTest()
    {
        // We are not testing the ApplyCommand
        // which is why we pass null
        // We are only testing the logic of 
        // TotalServing = CountOfServing * ServingDivider
        var vm = new PortionDialogVm.IntoSeveralEqualPartsVm(null!, null!, null!)
        {
            TotalServing = 10,
            CountOfServing = 5
        };

        // 10 = 5 * ServingDivider
        // ServingDivider = 10 / 5 = 2

        vm.CountOfServing = 13;

        // 10 = 13 * ServingDivider
        // ServingDivider = 10 / 13 = 0.769

        Assert.AreEqual(0.769, vm.ServingDivider);
    }

    [TestMethod]
    public void IntoSeveralEqualPartChangingServingDividerTest()
    {
        // We are not testing the ApplyCommand
        // which is why we pass null
        // We are only testing the logic of 
        // TotalServing = CountOfServing * ServingDivider
        var vm = new PortionDialogVm.IntoSeveralEqualPartsVm(null!, null!, null!)
        {
            TotalServing = 10,
            CountOfServing = 5,

            // 10 = 5 * ServingDivider
            // ServingDivider = 10 / 5 = 2
            
            ServingDivider = 13.33

            // TotalServing = 5 * 13.33 = 66.65
        };


        Assert.AreEqual(66.65, vm.TotalServing);
    }
}
