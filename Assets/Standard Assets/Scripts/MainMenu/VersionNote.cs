
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antinomia.MainMenu {

    public class VersionNote {

        private string versionNumber;

        private string versionInformation;

        private bool updateNecessary;

        public string VersionNumber {
            get {
                return versionNumber;
            }

            set {
                versionNumber = value;
            }
        }

        public string VersionInformation {
            get {
                return versionInformation;
            }

            set {
                versionInformation = value;
            }
        }

        public bool UpdateNecessary {
            get {
                return updateNecessary;
            }

            set {
                updateNecessary = value;
            }
        }

        public VersionNote(string _VersionNumber, string _VersionInformation, bool _updateNecessary) {
            VersionNumber = _VersionNumber;
            VersionInformation = _VersionInformation;
            UpdateNecessary = _updateNecessary;
        }

        public VersionNote(string _VersionNumber, string _VersionInformation, string _updateNecessary) {
            VersionNumber = _VersionNumber;
            VersionInformation = _VersionInformation;
            if (_updateNecessary == "True" || _updateNecessary == "true") {
                UpdateNecessary = true;
            }
            else {
                UpdateNecessary = false;
            }
        }
    }

}
