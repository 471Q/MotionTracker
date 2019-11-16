  var database = firebase.database();

  database.ref().once('value', function(snapshot){
        if(snapshot.exists()){
            var content = '';
			var name;
			var highest = 0;
			var currHighest = [];
			var currName = [];
            snapshot.forEach(function(data){
                var val = data.val();
				var keys = Object.keys(val);
				
				for(var i = 0; i < keys.length; i++)
				{
					var k = keys[i];
					
					var my = "thrn";
					
					content +='<tr class="item">';
					content += '<td data-key="name" >' + val[k].Name + '</td>';
					content += '<td data-key="usrName" >' + val[k].Username + '</td>';					
					content += '<td data-key="age" >' + val[k].Age + '</td>';
					content += '<td data-key="height" >' + val[k].Height + '</td>';
					content += '<td data-key="weight" >' + val[k].Weight + '</td>';
					content += '<td data-key="points" >' + val[k].Points + '</td>';
					content += '<td align="center">' + '<button data-toggle="modal" data-target="#editModal" onClick="run()" type="button" class="btn btn-primary"><i class="fa fa-edit"></i></button>'+ ' ' + '<button type="button" data-toggle="modal" data-target="#delModal" onClick="run()" class="btn btn-danger"><i class="fa fa-trash"></button>' +'</td>';
					content += '</tr>';
					
					currHighest.push(val[k].Points);
					currName.push(val[k].Name);
					
				}
            });
            $('#myTable').append(content);
        }
    });
  
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
			document.getElementById("fage").value = cells[2].innerHTML;
			document.getElementById("fhgt").value = cells[3].innerHTML;
			document.getElementById("fwgt").value = cells[4].innerHTML;
			document.getElementById("fpts").value = cells[5].innerHTML;
			
			//delete modal para
			document.getElementById("delU").innerHTML = "Are you sure, you want to remove user: " + delUser;
			
			//deleting a user
			$(document).on("click", "#delBtn", function(){
				console.log("wordjb");
				
				let userRef = database.ref('Users/' + delUserName);
				userRef.remove()
				
				location.reload();
			});
			
			//updating a user
			$(document).on("click", "#edtBtn", function(){
				var name = document.getElementById("myName").value;
				var userName = document.getElementById("fusr").value;
				var age = document.getElementById("fage").value;
				var height = document.getElementById("fhgt").value;
				var weight = document.getElementById("fwgt").value;
				var points = document.getElementById("fpts").value;
				
				console.log(name + " " + age);
				
				let userRef = database.ref('Users/');
				
				userRef.child(userName).update({
					'Name': name,
					'Username': userName,
					'Age': age,
					'Height': height,
					'Weight': weight,
					'Points': points
				})
				location.reload();
			});
        };
	}
	
	
