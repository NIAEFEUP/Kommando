String.prototype.trim = function() {
    return this.replace(/^\s+|\s+$/g, "");
};

var speed=0.003;
var hitValue=1000;

function Player(posx, posy,color,game){
	this.posx=posx;
	this.posy=posy;
	this.color=color;
	this.game=game;
	this.kind='p';
	this.token=game+Math.random().toString(36).substring(4,8);
	this.points=0;
	this.shoot=0;
  this.looked=0;
  this.ws=null;
  this.deltaX=0;
  this.deltaY=0;
}
Player.prototype.exit=function(){
	if(games[this.game]!=null){
    games[this.game].players[this.color]=null;
  }
}

Player.prototype.move=function(deltaX,deltaY){
	//this.posx=this.posx+speed*deltaX;
  
	//this.posy=this.posy+speed*deltaY;
  console.log('Deltas '+deltaX+','+deltaY)
  if(isNaN(deltaX) || isNaN(deltaY)){
    return;
  }
  this.deltaX=Number(deltaX);
  this.deltaY=Number(deltaY);
}

Player.prototype.shooting=function(){
	if(this.looked==1){
    return;
  }
  this.shoot=1;
  this.looked=1;
}

Player.prototype.miss=function(){
  this.ws.send('$,miss,'+this.points+','+games[this.game].damage);
  this.looked=0;
}

Player.prototype.avoid=function(){
  this.ws.send('$,avoided,'+this.points+','+games[this.game].damage);
  this.looked=0;
}



Player.prototype.update=function(){
  this.ws.send('$,update,'+this.points+','+games[this.game].damage);
}

Player.prototype.hit=function(){
  this.looked=0;
  this.points=this.points+hitValue;
  this.ws.send('$,hit,'+this.points+','+games[this.game].damage);
}

function Viewer(){
	this.ws=null;
  this.socket=null;
}

function Game()
{
	this.token=Math.random().toString(36).substring(4,8);
	this.players=new Array(6);
	this.view=new Viewer();
	this.kind='v';
  this.damage=0;
}

Game.prototype.addPlayer= function(){
	for(var i=0; i<this.players.length;i++){
		if(this.players[i]==null){
			var p=new Player(0.5,0.5,i,this.token);
			this.players[i]=p;
			return p;
		}
	}
	return null;

}

Game.prototype.process=function(data){
  var data=data.split(',');
  console.log('data from viwer ' + this.token);
  if(data.length==this.players.length+1){
    console.log('valid data from '+this.token);
    var i=0;
    this.damage=Number(data[this.players.length]);
    for(i=0;i<this.players.length;i++){
      if(this.players[i]!=null){
        if(this.players[i].ws.readyState!=this.players[i].ws.OPEN){
          this.players[i].exit();
        }
        if(Number(data[i])==1){
          this.players[i].miss();
        }else if(Number(data[i])==2){
          this.players[i].hit();
        }else if(Number(data[i])==3){
          this.players[i].avoid();
        }else{
          this.players[i].update();
        }
      }
    }
  }
}


function sendData(){
	var keys=Object.keys(games);
	for(var j=0; j<keys.length;j++){
		var a=games[keys[j]];
		var data="";
		for(var i=0; i< a.players.length;i++){
			if(a.players[i]==null){
				data=data+"-1,-1,-1,-1";
			}else{
				data=data+a.players[i].posx+","+a.players[i].posy+","+a.players[i].shoot+","+a.players[i].points;
			  a.players[i].shoot=0;
      }
      if(i<a.players.length-1){
        data=data+',';
      }
		}
    if(a.view.socket!=null){
      //a.view.socket.write(data+'\n');
      console.log('tcp socket unactive');
    }else{
      if(a.view.ws.readyState==a.view.ws.OPEN){
		    a.view.ws.send(data);
      }
      //console.log(data);
    }
	}
}
timer=setInterval(sendData,50);

function updatePlayers(){
 var playerk=Object.keys(players); 
 for(var i=0; i<playerk.length; i++){
  //console.log('before:');
  //console.log(players[playerk[i]].posx);
  //console.log(players[playerk[i]].posy);
  //console.log(players[playerk[i]].deltaX);
  //console.log(players[playerk[i]].deltaY);
  //console.log(players[playerk[i]].deltaX*speed);
  players[playerk[i]].posx=players[playerk[i]].posx+speed*Number(players[playerk[i]].deltaX);
  players[playerk[i]].posy=players[playerk[i]].posy+speed*Number(players[playerk[i]].deltaY);
 // players[playerk[i]].posx=Number(players[playerk[i]].posx).toFixed(3);
 // players[playerk[i]].posy=Number(players[playerk[i]].posy).toFixed(3);

  //console.log('after');
  //console.log(players[playerk[i]].posx);
  //console.log(players[playerk[i]].posy);
  //console.log(players[playerk[i]].deltaX);
  //console.log(players[playerk[i]].deltaY);
  
  if(players[playerk[i]].posx>0.9){
  	players[playerk[i]].posx=0.9;
  }
  if(players[playerk[i]].posx<0.1){
  	players[playerk[i]].posx=0.1;
  }
  if(players[playerk[i]].posy>0.9){
  	players[playerk[i]].posy=0.9;
  }
  if(players[playerk[i]].posy<0.15){
  	players[playerk[i]].posy=0.15;
  }
 }

}

var timer2=setInterval(updatePlayers,50);



var games=new Object();
var players=new Object();

var WebSocketServer = require('ws').Server
  , wss = new WebSocketServer({port: 1234});
wss.on('connection', function(ws) {
   console.log('connect');
    ws.on('message', function(message) {
	if(this.who==null){
		if(message[0]=='p'){
			var a=message.split(',');
			if(a.length>1){
				var token=a[1].trim();
				console.log(token);
				var g= games[token];
				if(g!=null){
					var p=g.addPlayer();
					if(p!=null){
						players[p.token]=p;
						this.who=p;
            p.ws=this;
						this.send('!,'+this.who.token+','+this.who.color);
					}else{
						this.send('full');
					}
				}else{
          this.send('noGame');
        }
			}

		}else if(message=='v'){
			this.who=new Game();
			this.who.view.ws=this;
			games[this.who.token]=this.who;
			this.send(this.who.token);
		}
	}else{
		if(this.who.kind=="p"){
			if(message[0]=='s'){
				this.who.shooting();
			}else{
				var a=message.split(',');
				if(a.length==2){
					this.who.move(Number(a[0]),Number(a[1]));
				}
			}
		}
	}
	if(this.who!=null){
		console.log(this.who.kind);
		console.log(this.who.token);
    if(this.who.kind=='v'){
      this.who.process(message);
    } 
	}
        console.log('received: %s', message);
    });
    ws.on('close', function() {
    	console.log('disconnected');
	    if(this.who==null){
        return;
      }
      if(this.who.kind=="v"){
		   if(this.who!=null){
			console.log(this.who.token+" out.");
		    if(this.who.kind=="v"){
		      console.log('exit players');
				  for(var i=0;i<games[this.who.token].players.length;i++){
		        var p=games[this.who.token].players[i];
		        console.log('Player '+i+':' );
		        if(p!=null){
		          console.log('ok');
		          p.ws.send('end');
		          p.ws.close();
		          delete players[p.token];
		          delete p;
		        }
		      }
		      delete games[this.who.token];
			  }
			  delete this.who;
			  return;
		  	}
	}else if(this.who.kind=="p"){
		this.who.exit();
		delete players[this.who.token];
	}
	delete this.who;
    });

    ws.who=null;
    ws.send('whoareyou');
});

