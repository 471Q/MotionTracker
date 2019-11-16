var database = firebase.database();

database.ref().once('value', function(snapshot){
	if(snapshot.exists()){
		var name;
		var highest = 0;
		var location = 1;
		var currHighest = [];
		var currName = [];
		snapshot.forEach(function(data){
			var val = data.val();
			var keys = Object.keys(val);
			
			for(var i = 0; i < keys.length; i++)
			{
				var k = keys[i];
				
				currHighest.push(val[k].Points);
				currName.push(val[k].Name);				
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
		});
	}
});