$(document).ready(function(){
  $("#abrir_Menu").click(function(){
	 $("#menu").animate({"right":"0"});
   });
  $("#cerrar_Menu").click(function(){
	 $("#menu").animate({"right":"-100%"});
  });
    $(".close").click(function () {
        $("#AlertDanger").alert("close");
        $("#AlertSuccess").alert("close");
        $("#AlertWarning").alert("close");
    });
});
