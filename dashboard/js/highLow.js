var database = firebase.database();

database.ref().once('value', function(snapshot){
	if(snapshot.exists()){
		var name;
		var highest = 0;
		var location = 1;
		var currHighest = [];
		var currName = [];
		var percTask = [];
		snapshot.forEach(function(data){
			var val = data.val();
			var keys = Object.keys(val);
			
			for(var i = 0; i < keys.length; i++)
			{
				var k = keys[i];
				
				currHighest.push(val[k].Points);
				currName.push(val[k].Name);	
				var perc = val[k].Points*100/val[k].MaxPoints;
				percTask.push(perc);
				console.log(perc);
			}

			
			//getting minimum
			minimum = currHighest[0];
   
			for (c = 1; c < currHighest.length; c++)
			{
				if (currHighest[c] < minimum)
				{
				   minimum = currHighest[c];
				   location = c;
				   
				//   console.log(minimum);
				}
			}
			document.getElementById("lpe").innerHTML = currName[location] + " : " + minimum;
			
			//getting maximum
			maximum = currHighest[0];
   
			for (c = 1; c < currHighest.length; c++)
			{
				if (currHighest[c] > maximum)
				{
				   maximum = currHighest[c];
				   location = c;
				}
			}
			document.getElementById("hpe").innerHTML = currName[location] + " : " + maximum;
			
			//getting minimum task completed
			minimum = percTask[0];
   
			for (c = 1; c < percTask.length; c++)
			{
				if (percTask[c] < minimum)
				{
				   minimum = percTask[c];
				   location = c;
				   
				//   console.log(minimum);
				}
			}
			document.getElementById("mTask").style.width = minimum;
			document.getElementById("mTaskNmbr").innerHTML = minimum+"%";
			document.getElementById("mTaskTxt").innerHTML = "Least Task Completed: " + currName[location];
			
			//getting maximum task completed
			maximum = percTask[0];
   
			for (c = 1; c < percTask.length; c++)
			{
				if (percTask[c] > maximum)
				{
				   maximum = percTask[c];
				   location = c;
				}
			}
			document.getElementById("hTask").style.width = maximum+"%";
			document.getElementById("hTaskNmbr").innerHTML = maximum + "%";
			document.getElementById("hTaskTxt").innerHTML = "Most Task Completed: " + currName[location];
			
		});
	}
});