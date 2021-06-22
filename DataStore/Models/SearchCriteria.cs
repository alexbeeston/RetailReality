using System;
using System.Collections.Generic;
using System.Text;

namespace DataStore
{
	public class SearchCriteria
	{
		public string department;
		public string category;
		public string product;
		public string silhouette;
		public string occasion;
		public Gender gender;
		public int id;

		public override string ToString()
		{
			string genderString = gender == Gender.Female ? "female" : "male";
			return $"\"Department: {department}, Category: {category}, Product: {product}, Silhouette: {silhouette}, Occasion: {occasion}, Gender: {genderString}, Id: {id}\"";
		}
	}
}
