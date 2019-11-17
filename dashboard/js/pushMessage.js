  var database = firebase.database();

  database.ref().once('value', function(snapshot){
        if(snapshot.exists()){
            snapshot.forEach(function(data){
                var val = data.val();
				var keys = Object.keys(val);
				
				for(var i = 0; i < keys.length; i++)
				{
					var k = keys[i];
						
					content +='<tr class="item">';
					content += '<td data-key="name" >' + val[k].Name + '</td>';
					content += '<td data-key="usrName" >' + val[k].Username + '</td>';
					content += '<td>' + val[k].Message + '</td>';							
					content += '<td align="center">' + '<button data-toggle="modal" data-target="#editModal" onClick="run()" type="button" class="btn btn-primary">Send Message <i class="fa fa-paper-plane"></i></button>' +'</td>';
					content += '</tr>';
				}
            });
            $('#myTable').append(content);
        }
    });

  var points = [];
  
  function gotdata(data)
  {
	// console.log(data.val());
	var userName = data.val();
	
	var keys = Object.keys(userName);
	
	//console.log(keys);
	
	for(var i = 0; i < keys.length; i++)
	{
		var k = keys[i];
		
		var name = userName[k].Name;
		
		console.log(name);
	}
  }
  
    function run() {
      	var t = document.getElementById('myTable');
        t.onclick = function (event) {
            event = event || window.event; //IE8
            var target = event.target || event.srcElement;
            while (target && target.nodeName != 'TR') { // find TR
                target = target.parentElement;
            }
            //if (!target) { return; } //tr should be always found
            var cells = target.cells; //cell collection - https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableRowElement
            //var cells = target.getElementsByTagName('td'); //alternative
            if (!cells.length || target.parentNode.nodeName == 'THEAD') {
                return;
            }
        
            
			var delUser = cells[0].innerHTML;
			var delUserName = cells[1].innerHTML;
			
			
			//console.log(delUser);
			
			//set edit form-values
			document.getElementById("myName").value = cells[0].innerHTML;
			document.getElementById("fusr").value = cells[1].innerHTML;
			document.getElementById("fmsg").value = cells[2].innerHTML;

			
		
			//updating a user
			$(document).on("click", "#edtBtn", function(){
				var name = document.getElementById("myName").value;
				var userName = document.getElementById("fusr").value;
				var msg = document.getElementById("fmsg").value;
	
				
				console.log(name + " " + msg);
			 	
				let userRef = database.ref('Users/');
				
				userRef.child(userName).update({
					'Name': name,
					'Username': userName,
					'Message': msg,
				}) 
				location.reload();
			});
        };
	}
	
	
