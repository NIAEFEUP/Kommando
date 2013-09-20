$(document).ready(function(){
	$('#join').click(function(){
		if ("WebSocket" in window){
			window.location.href=window.location.href.substring(0,window.location.href.lastIndexOf('/')+1)+'game.html?game='+$('#token').val();
		}else{
			alert('Browser not supported');
		}
	});

});