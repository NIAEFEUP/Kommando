var colors=['#25ADDF',"#b268d9","#8abd00","#ffa712","#f83a3a","#17a086"];
var wsurl="ws://192.168.27.133:1234";

function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split('&');
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split('=');
        if (decodeURIComponent(pair[0]) == variable) {
            return decodeURIComponent(pair[1]);
        }
    }
    console.log('Query variable %s not found', variable);
    return null;
}

$(window).bind("scroll",function(evt){console.log(evt);evt.preventDefault();});

function setPlayerColor(p){
	if(p>5){
		return;
	}
	if(p<0){
		return;
	}
	
	$('#color').css('background-color',colors[p]);
	$('#outer').css('background-color',colors[p]);
	$('#inner').css('background-color',colors[p]);
}

var ft=function(evt){

    	evt.preventDefault();

		var x=($('#joystick').position().left+$('#joystick').outerWidth(true)-$('#joystick').outerWidth()/2.0);
		var y=($('#joystick').position().top+$('#joystick').outerHeight(true)-$('#joystick').outerHeight()/2.0);
		var deltaY=evt.originalEvent.touches[0].pageY-y;
		var deltaX=evt.originalEvent.touches[0].pageX-x;
		if(deltaX>($('#joyout').innerWidth()/2.0)){
			deltaX=($('#joyout').innerWidth()/2.0);
		}
		if(deltaY>($('#joyout').innerHeight()/2.0)){
			deltaY=($('#joyout').innerHeight()/2.0);
		}
		deltaX=deltaX/10;
		deltaX=deltaX.toFixed(3);
		deltaY=deltaY/10;
		deltaY=deltaY.toFixed(3);
		$('#debug').html(deltaX+' , '+deltaY);
		console.log(deltaX+' , '+deltaY);

		if(ws){
			ws.send(deltaX+','+deltaY);
		}
		//$('#joystick').attr('style','-webkit-transform: translate('+deltaX+'px,'+deltaY+'px);');
	};

var deltaX=-360;
var deltaY=-360;

function onMessage(msg){
	msg=msg.data;
	console.log(msg);
	if(msg.length==0){
		return;
	}
	console.log(msg[0] + " igual a " + (msg[0]=='!'));
	if(msg=='whoareyou'){
		cout('ready to join');
		if(token!=null){
			$('#join').click();
		}
	}else{
		 if(msg[0]=='!'){
			console.log('join');
			var s=msg.split(',');
			if(s.length!=3){
				return;
			}
			cout(s[1]);
			setPlayerColor(Number(s[2]));
			console.log(s[2]+' - ' +Number(s[2]));
		}else{
			if(msg[0]=='$'){
				console.log('join');
				var s=msg.split(',');
				if(s.length!=4){
					return;
				}
				cout(s[1]);
				setPoints(Number(s[2]));
				setDamage(Number(s[3])*100);	
			}else{
				if(msg=='end'){
					backToStart();
				}else{
					if(msg=='full'){
						cout('Game is full');
						alert('Game is full');
						backToStart();
					}else if(msg='noGame'){
						cout('Wrong Game!');
						alert('Wrong Game!');
						backToStart();
					}
				}
			}
		}
	}

	
}

function backToStart(){
	//window.location.href=window.location.href.substring(0,window.location.href.lastIndexOf('/')+1);
}

function setPoints(val){
	$("#points").html(val);
}

function setDamage(val){
	$("#damage").html(val+'%');
}

function cout(msg){
	$('#data').html(msg);
}
var ws;

function onClose(evt){
	backToStart();
}


function connect(){
	if ("WebSocket" in window){
		ws=new WebSocket(wsurl);
		if(ws==null){
			cout('not connected to server');
		}else{
			cout('websocket supported');
			ws.onmessage = function(evt) { onMessage(evt) };
			ws.onclose = function(evt) { onClose(evt) };
			ws.onerror = function(evt) { onClose(evt) };
		}
	}else{
		cout('not supported');
	}
}

var token;

$(document).ready(function(){
	
	/*$('#joystick').attr('x',($('#joystick').position().left+$('#joystick').outerWidth(true)-$('#joystick').outerWidth()/2.0));
	$('#joystick').attr('y',($('#joystick').position().top+$('#joystick').outerHeight(true)-$('#joystick').outerHeight()/2.0));*/
	$('#joystick').bind('touchmove',ft);
	$('#joystick').bind('touchstart',ft);
	$('#token').bind('touchstart',function(evt){
		$('#debug').html("token");
	});
  if (window.DeviceOrientationEvent) {
    window.addEventListener("deviceorientation", function( event ) {
	//alpha: rotation around z-axis
	  var rotateDegrees = event.alpha;
	//gamma: left to right
	  var leftToRight = event.gamma;
	//beta: front back motion
	  var frontToBack = event.beta;
				 
	  handleOrientationEvent( frontToBack, leftToRight, rotateDegrees );  }, false);
  }
	$(document).bind('touchend',function(evt){
		evt.preventDefault();
		$('#joystick').attr('style','');
		if(ws){
			ws.send('0,0');
		}
	});
	$('#fire').bind('touchend',function(evt){
		evt.preventDefault();
		$('#debug').html($('#debug').html()+" fire");
		if(ws){
			ws.send('s');
				if(window.navigator.mozVibrate){
				window.navigator.mozVibrate([500]);
			}else{
				if(window.navigator.webkitVibrate){
				window.navigator.mozVibrate([500]);
			}else{
				if(window.navigator.vibrate){
				window.navigator.vibrate([500]);
			}
			}
			}
		}
	});
	$(document).bind('touchend',function(evt){
		evt.preventDefault();
		$('#joystick').attr('style','');
		if(ws){
			ws.send('0,0');
		}
	});
	$(window).bind("scroll",function(evt){console.log(evt);evt.preventDefault();});
	console.log('ready');
	$('html, body').css('overflow-y', 'hidden');
	connect();
	token=getQueryVariable('game');
	$('#connect').click(function(){
		
	});
	$('#join').click(function(){
		ws.send('p,'+token);
	});
	if(token!=null){
		//$('#join').click();
	}else{
		alert('No Game Selected');
		backToStart();
	}
});

  var initialAlpha=-360;
  var initialBeta=-360;

var handleOrientationEvent = function( frontToBack, leftToRight, rotateDegrees ){
    
    //frontToBack=frontToBack+180;
    if(initialBeta<-359 || deltaY<-359){
     initialAlpha=leftToRight; 
      initialBeta=frontToBack;
      return true;
    }
    var deltaX=leftToRight-initialAlpha;
    var deltaY=initialBeta-frontToBack;
    deltaX=deltaX/10.0; 
    deltaX=deltaX.toFixed(2);
    deltaY=deltaY.toFixed(2);
    cout(deltaX+","+deltaY);
    ws.send(deltaX+","+deltaY);
};



