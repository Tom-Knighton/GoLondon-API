using System;
using System.Xml.Serialization;
using GoLondonAPI.Domain.Enums;

namespace GoLondonAPI.Domain.Models
{

	public class StopPointAccessibility
    {
		public string StationName { get; }
		public StationAccessibilityType OverviewAccessibility { get; }

		public List<StopPointLineAccessibility> LineAccessibility { get; }


		public class StopPointLineAccessibility
        {
			public string LineName { get; set; }
			public string LineDirection { get; set; }
			public StationAccessibilityType Accessibility { get; set; }
        }

		public enum StationAccessibilityType
        {
			StepFreeToTrain,
			StepFreeToToPlatform,
			Partial,
			Interchange,
			None
        }

		public StopPointAccessibility(AccessibilityLink fromLink)
        {
			this.StationName = fromLink.StationName;

			List<StopPointLineAccessibility> lines = new();

			foreach(ALLines lineGroup in fromLink.Lines)
            {
				foreach (ALLine line in lineGroup.Line)
                {
					bool hasZeroGap = line.StepMin == 0 && line.GapMin == 0;
					bool hasManualRamp = line.LevelAccessByManualRamp.Count > 0;
					lines.Add(new StopPointLineAccessibility
					{
						LineName = line.LineName,
						LineDirection = line.Direction,
						Accessibility = hasZeroGap && !hasManualRamp ? StationAccessibilityType.StepFreeToTrain : hasZeroGap ? StationAccessibilityType.StepFreeToToPlatform : StationAccessibilityType.None
					}); ;
                }
            }

			this.LineAccessibility = lines;
			bool hasAllStepFreeToTrain = lines.All(l => l.Accessibility == StationAccessibilityType.StepFreeToTrain);
			bool hasAnyStepFreeToPlatform = lines.Any(l => l.Accessibility == StationAccessibilityType.StepFreeToToPlatform);
			this.OverviewAccessibility = hasAllStepFreeToTrain ? StationAccessibilityType.StepFreeToTrain : hasAnyStepFreeToPlatform ? StationAccessibilityType.StepFreeToToPlatform : StationAccessibilityType.None;
			this.OverviewAccessibility = fromLink.AccessibilityType == "Partial" ? StationAccessibilityType.Partial : fromLink.AccessibilityType == "Interchange" ? StationAccessibilityType.Interchange : this.OverviewAccessibility;
		}
    }

    [XmlRoot(ElementName = "Stations")]
    public class AccessibilityLinkRoot
    {
        [XmlElement(ElementName = "Station")]
        public List<AccessibilityLink> Stations { get; set; }
    }
    public class AccessibilityLink
    {
		[XmlElement(ElementName = "StationName")]
		public string StationName { get; set; }
		[XmlElement(ElementName = "AccessibilityType")]
        public string AccessibilityType { get; set; }
        [XmlElement(ElementName = "Lines")]
        public List<ALLines> Lines { get; set; }
        [XmlElement(ElementName = "NaptanCode")]
        public string NaptanCode { get; set; }


        
	}

	[XmlRoot(ElementName = "Lines")]
	public class ALLines
	{
		[XmlElement(ElementName = "AdditionalAccessibilityInformation")]
		public string AdditionalAccessibilityInformation { get; set; }
		[XmlElement(ElementName = "Line")]
		public List<ALLine> Line { get; set; }
	}

	[XmlRoot(ElementName = "Line")]
	public class ALLine
	{
		[XmlElement(ElementName = "AccessibilityType")]
		public string AccessibilityType { get; set; }
		[XmlElement(ElementName = "AdditionalAccessibilityInformation")]
		public string AdditionalAccessibilityInformation { get; set; }
		[XmlElement(ElementName = "DesignatedLevelAccessPoint")]
		public string DesignatedLevelAccessPoint { get; set; }
		[XmlElement(ElementName = "Direction")]
		public string Direction { get; set; }
		[XmlElement(ElementName = "DirectionTowards")]
		public string DirectionTowards { get; set; }
		[XmlElement(ElementName = "GapAverage")]
		public int GapAverage { get; set; }
		[XmlElement(ElementName = "GapMax")]
		public int GapMax { get; set; }
		[XmlElement(ElementName = "GapMin")]
		public int GapMin { get; set; }
		[XmlElement(ElementName = "LevelAccessByManualRamp")]
		public List<string> LevelAccessByManualRamp { get; set; }
		[XmlElement(ElementName = "LineName")]
		public string LineName { get; set; }
		[XmlElement(ElementName = "LocationOfLevelAccess")]
		public string LocationOfLevelAccess { get; set; }
		[XmlElement(ElementName = "Platform")]
		public string Platform { get; set; }
		[XmlElement(ElementName = "SpecificEntranceRequired")]
		public string SpecificEntranceRequired { get; set; }
		[XmlElement(ElementName = "StepAverage")]
		public int StepAverage { get; set; }
		[XmlElement(ElementName = "StepMax")]
		public int StepMax { get; set; }
		[XmlElement(ElementName = "StepMin")]
		public int StepMin { get; set; }
	}
}

