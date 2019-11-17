var database = firebase.database();

window.onload = function(){
	database.ref().once('value', function(snapshot){
	if(snapshot.exists()){
		var name;
		var highest = 0;
		var location = 0;
		var currHighest = [];
		var currName = [];
		var percTask = [];
		var myMap = new Map();
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
				
				var name = val[k].Name;
				
				myMap.set(name,perc);
			}
			//console.log(myMap.get('aaa'))
			for (let [k, v] of myMap) {
				//console.log("Key: " + k);
				//console.log("Value: " + v);
			}
			
		//	console.log("Max:", Math.max(...myMap.values()));
		//	console.log("Min:", Math.min(...myMap.values()));
			
			//getting minimum
			minimum = currHighest[0];
   
			for (c = 0; c <= currHighest.length; c++)
			{
				if (currHighest[c] < minimum)
				{
				   minimum = currHighest[c];
				   location = c;
				}
			}
			document.getElementById("lpe").innerHTML = currName[location] + " : " + minimum;
			
			//getting maximum
			maximum = currHighest[0];
   
			for (c = 0; c < currHighest.length; c++)
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
   
			for (c = 0; c < percTask.length; c++)
			{
				if (percTask[c] <= minimum)
				{
				   minimum = percTask[c];
				   location = c;
				}
			}
			document.getElementById("mTask").style.width = Math.floor(minimum)+"%";
			document.getElementById("mTaskNmbr").innerHTML = Math.floor(minimum)+"%";
			document.getElementById("mTaskTxt").innerHTML = "Least Task Completed: " + currName[location];
			
			//getting maximum task completed
			maximum = percTask[0];
   
			for (c = 0; c < percTask.length; c++)
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
			
			
			//sorted Task 
			myMap[Symbol.iterator] = function* () {
				yield* [...this.entries()].sort((a, b) => a[1] - b[1]);
			}
			count = 1;
			for (let [key, value] of myMap) {     // get data sorted
				if(count <= 5)
				{
					document.getElementById("n"+count).innerHTML = key + "<span class='float-right'>" + Math.floor(value) + "%" + "</span>";
					document.getElementById("b"+count).style.width = Math.floor(value) + "%";
				}
				count++;
			}
		});
	}
});
}


