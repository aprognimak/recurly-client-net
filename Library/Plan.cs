﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;

namespace Recurly
{
    public class Plan
    {
        public enum IntervalUnit
        {
            days,
            months
        }

        public string PlanCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }

        public bool? DisplayDonationAmounts { get; set; }
        public bool? DisplayQuantity { get; set; }
        public bool? DisplayPhoneNumber { get; set; }
        public bool? BypassHostedConfirmation { get; set; }

        public string UnitName { get; set; }
        public string PaymentPageTOSLink { get; set; }

        public int PlanIntervalLength { get; set; }
        public IntervalUnit PlanIntervalUnit { get; set; }

        public int TrialIntervalLength { get; set; }
        public IntervalUnit TrialIntervalUnit { get; set; }

        public string AccountingCode { get; set; }

        public DateTime CreatedAt { get; private set; }

        public int? TotalBillingCycles { get; set; }

        public AddOnList AddOns { get; private set; }


        private Dictionary<string, int> _unitAmountInCents;
        /// <summary>
        /// A dictionary of currencies and values for the subscription amount
        /// </summary>
        public Dictionary<string, int> UnitAmountInCents
        {
            get
            {
                if (null == _unitAmountInCents)
                {
                    _unitAmountInCents = new Dictionary<string, int>();
                }
                return _unitAmountInCents;
            }
        }

        private Dictionary<string, int> _setupFeeInCents;
        /// <summary>
        /// A dictionary of currency and values for the setup fee
        /// </summary>
        public Dictionary<string, int> SetupFeeInCents
        {
            get
            {
                if (null == _setupFeeInCents)
                    _setupFeeInCents = new Dictionary<string, int>();

                return _setupFeeInCents;
            }

        }

        internal const string UrlPrefix = "/plans/";

        #region Constructors
        internal Plan()
        {
        }

        internal Plan(XmlTextReader xmlReader)
        {
            ReadXml(xmlReader);
        }

        public Plan(string planCode, string name)
        {
            this.PlanCode = planCode;
            this.Name = name;

        }

        #endregion

        /// <summary>
        /// Retrieves a Plan
        /// </summary>
        /// <param name="planCode"></param>
        /// <returns></returns>
        public static Plan Get(string planCode)
        {
            Plan plan = new Plan();

            HttpStatusCode statusCode = Client.PerformRequest(Client.HttpRequestMethod.Get,
                UrlPrefix + System.Uri.EscapeUriString(planCode),
                new Client.ReadXmlDelegate(plan.ReadXml));

            if (statusCode == HttpStatusCode.NotFound)
                return null;

            return plan;
        }

        /// <summary>
        /// Create a new plan in Recurly
        /// </summary>
        public void Create()
        {
            Client.PerformRequest(Client.HttpRequestMethod.Post,
                UrlPrefix,
                new Client.WriteXmlDelegate(this.WriteXml));
        }

        /// <summary>
        /// Update an existing plan in Recurly
        /// </summary>
        public void Update()
        {
            Client.PerformRequest(Client.HttpRequestMethod.Put,
                UrlPrefix + System.Uri.EscapeUriString(this.PlanCode),
                new Client.WriteXmlDelegate(this.WriteXml));
        }

        /// <summary>
        /// Deletes this plan, making it inactive
        /// </summary>
        public void Deactivate()
        {
            Client.PerformRequest(Client.HttpRequestMethod.Delete, UrlPrefix + System.Uri.EscapeUriString(this.PlanCode));
        }


        

        /// <summary>
        /// Returns an new add on associated with this plan.
        /// </summary>
        /// <param name="addOnCode"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public AddOn CreateAddOn(string addOnCode, string name)
        {
            AddOn a = new AddOn(this.PlanCode, addOnCode, name);
            return a;
        }

        #region Read and Write XML documents

        internal void ReadXmlSetupFee(XmlTextReader reader)
        {

            while (reader.Read())
            {
                if (reader.Name == "setup_fee_in_cents" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    this.SetupFeeInCents.Add(reader.Name, reader.ReadElementContentAsInt());
                }
            }
        }

        internal void ReadXmlUnitAmount(XmlTextReader reader)
        {

            while (reader.Read())
            {
                if (reader.Name == "unit_amount_in_cents" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    this.UnitAmountInCents.Add(reader.Name, reader.ReadElementContentAsInt());
                }
            }
        }

        internal void ReadXml(XmlTextReader reader)
        {
            while (reader.Read())
            {
                // End of account element, get out of here
                if (reader.Name == "plan" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {

                        case "plan_code":
                            this.PlanCode = reader.ReadElementContentAsString();
                            break;

                        case "name":
                            this.Name = reader.ReadElementContentAsString();
                            break;

                        case "description":
                            this.Description = reader.ReadElementContentAsString();
                            break;

                        case "success_url":
                            this.SuccessUrl = reader.ReadElementContentAsString();
                            break;

                        case "cancel_url":
                            this.CancelUrl = reader.ReadElementContentAsString();
                            break;

                        case "display_donation_amounts":
                            this.DisplayDonationAmounts = reader.ReadElementContentAsBoolean();
                            break;

                        case "display_quantity":
                            this.DisplayQuantity = reader.ReadElementContentAsBoolean();
                            break;

                        case "display_phone_number":
                            this.DisplayPhoneNumber = reader.ReadElementContentAsBoolean();
                            break;

                        case "bypass_hosted_confirmation":
                            this.BypassHostedConfirmation = reader.ReadElementContentAsBoolean();
                            break;

                        case "unit_name":
                            this.UnitName = reader.ReadElementContentAsString();
                            break;

                        case "payment_page_tos_link":
                            this.PaymentPageTOSLink = reader.ReadElementContentAsString();
                            break;

                        case "plan_interval_length":
                            this.PlanIntervalLength = reader.ReadElementContentAsInt();
                            break;

                        case "plan_interval_unit":
                            this.PlanIntervalUnit = (IntervalUnit)Enum.Parse(typeof(IntervalUnit), reader.ReadElementContentAsString(), true);
                            break;

                        case "trial_interval_length":
                            this.TrialIntervalLength = reader.ReadElementContentAsInt();
                            break;

                        case "trial_interval_unit":
                            this.TrialIntervalUnit = (IntervalUnit)Enum.Parse(typeof(IntervalUnit), reader.ReadElementContentAsString(), true);
                            break;

                        case "accounting_code":
                            this.AccountingCode = reader.ReadElementContentAsString();
                            break;

                        case "created_at":
                            this.CreatedAt = reader.ReadElementContentAsDateTime();
                            break;

                        case "unit_amount_in_cents":
                            ReadXmlUnitAmount(reader);
                            break;

                        case "setup_fee_in_cents":
                            ReadXmlSetupFee(reader);
                            break;
                    }
                }
            }
        }

        internal void WriteXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("plan");

            xmlWriter.WriteElementString("plan_code", this.PlanCode);
            xmlWriter.WriteElementString("name", this.Name);
            if (null != this.Description && this.Description.Length > 0)
                xmlWriter.WriteElementString("description", this.Description);
            if (null != this.AccountingCode && this.AccountingCode.Length > 0)
                xmlWriter.WriteElementString("accounting_code", this.AccountingCode);
            if (this.PlanIntervalLength > 0)
            {
                xmlWriter.WriteElementString("plan_interval_unit", this.PlanIntervalUnit.ToString());
                xmlWriter.WriteElementString("plan_interval_length", this.PlanIntervalLength.ToString());
            }
            if (this.TrialIntervalLength > 0)
            {
                xmlWriter.WriteElementString("trial_interval_unit", this.TrialIntervalUnit.ToString());
                xmlWriter.WriteElementString("trial_interval_length", this.TrialIntervalLength.ToString());
            }
            if (null !=  this.SetupFeeInCents &&  this._setupFeeInCents.Count > 0)
            {
                xmlWriter.WriteStartElement("setup_fee_in_cents");
                foreach (KeyValuePair<string, int> d in this.SetupFeeInCents)
                {
                    xmlWriter.WriteElementString(d.Key, d.Value.ToString());
                }
                xmlWriter.WriteEndElement();
            }

            if (null != this.UnitAmountInCents && this._unitAmountInCents.Count > 0)
            {
                xmlWriter.WriteStartElement("unit_amount_in_cents");
                foreach (KeyValuePair<string, int> d in UnitAmountInCents)
                {
                    xmlWriter.WriteElementString(d.Key, d.Value.ToString());
                }
                xmlWriter.WriteEndElement();
            }

            if (this.TotalBillingCycles > 0)
                xmlWriter.WriteElementString("total_billing_cycles", this.TotalBillingCycles.ToString());
            if (null != this.UnitName && this.UnitName.Length > 0)
                xmlWriter.WriteElementString("unit_name", this.UnitName);

            if (null != this.DisplayDonationAmounts && this.DisplayDonationAmounts.HasValue)
                xmlWriter.WriteElementString("display_donation_amounts", this.DisplayDonationAmounts.ToString());

            if (null != this.DisplayQuantity && this.DisplayQuantity.HasValue)
                xmlWriter.WriteElementString("display_quantity", this.DisplayQuantity.ToString());

            if (null != this.DisplayPhoneNumber && this.DisplayPhoneNumber.HasValue)
                xmlWriter.WriteElementString("display_phone_number", this.DisplayPhoneNumber.ToString());

            if (null != this.BypassHostedConfirmation && this.BypassHostedConfirmation.HasValue)
                xmlWriter.WriteElementString("bypass_hosted_confirmation", this.BypassHostedConfirmation.ToString());

            if (null != this.SuccessUrl && this.SuccessUrl.Length > 0)
                xmlWriter.WriteElementString("success_url", this.SuccessUrl);
            if (null != this.CancelUrl && this.CancelUrl.Length > 0)
                xmlWriter.WriteElementString("cancel_url", this.CancelUrl);

            xmlWriter.WriteEndElement();
        }


        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return "Recurly Plan: " + this.PlanCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is Plan)
                return Equals((Plan)obj);
            else
                return false;
        }

        public bool Equals(Plan plan)
        {
            return this.PlanCode == plan.PlanCode;
        }

        public override int GetHashCode()
        {
            return this.PlanCode.GetHashCode();
        }

        #endregion
    }
}
