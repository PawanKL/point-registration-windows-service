//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PointDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class PointFee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PointFee()
        {
            this.PointPayments = new HashSet<PointPayment>();
        }
    
        public int FeeID { get; set; }
        public int TransportFee { get; set; }
        public System.DateTime DueDate { get; set; }
        public int FineCharges { get; set; }
        public int SemesterID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PointPayment> PointPayments { get; set; }
    }
}
