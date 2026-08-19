using System;
using System.Collections.Generic;
using System.Text;

namespace AutoLotManager.Core
{
    /// <summary>
    /// Describes a single vehicle held in the dealership's inventory.
    /// </summary>
    /// <remarks>
    /// This is a plain data container. It performs no validation, raises no
    /// change notifications, and uses reference equality, so two instances
    /// describing the same physical vehicle are not considered equal.
    /// </remarks>
    public class Car
    {
        /// <summary>
        /// The vehicle identification number: the manufacturer-assigned code
        /// that uniquely identifies this vehicle. Accepted as-is; the value is
        /// neither validated nor normalised.
        /// </summary>
        public string Vin { get; set; }

        /// <summary>
        /// The manufacturer that built the vehicle, for example "Toyota".
        /// </summary>
        public string Make { get; set; }

        /// <summary>
        /// The manufacturer's model name for the vehicle, for example "Camry".
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The model year of the vehicle. Any integer is accepted; no range
        /// check is applied.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The exterior colour of the vehicle, held as free-form descriptive
        /// text rather than a value drawn from a fixed palette.
        /// </summary>
        public string Color { get; set; }

    }
}
