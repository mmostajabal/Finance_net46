$(document).ready(function () {
    alert("1");
    try{
        $('#PartnerTbl').DataTable({ "responsive": true, "paging": false, dom: 'Bfrtip', buttons: ['excel', 'print'] });
    }catch (e) {
        alert(e.toString());
    }
    alert("2");
});
