﻿using GroceryBama.Entities;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GroceryBama.MySqlScripts
{
    public class UsersScript : SqlConnector
    {
        public User RegisterBuyer(User user)
        {
            MySqlDataReader reader = GetStoredProcedureReader("RegisterBuyer",
                            new MySqlParameter("@p_username", user.Username),
                            new MySqlParameter("@p_firstname", user.Firstname),
                            new MySqlParameter("@p_lastname", user.Lastname),
                            new MySqlParameter("@p_password", user.Password),
                            new MySqlParameter("@p_phone", user.PhoneNumber),
                            new MySqlParameter("@p_email", user.Email),
                            new MySqlParameter("@p_street", user.StreetAddress),
                            new MySqlParameter("@p_city", user.City),
                            new MySqlParameter("@p_addressline2", user.AddressLine2),
                            new MySqlParameter("@p_zipcode", user.ZipCode),
                            new MySqlParameter("@p_state", user.State));
            reader.Close();
            return GetUserInfo(user.Username);
        }
        public User RegisterDeliverer(User user)
        {
            MySqlDataReader reader = GetStoredProcedureReader("RegisterDeliverer",
                            new MySqlParameter("@p_username", user.Username),
                            new MySqlParameter("@p_firstname", user.Firstname),
                            new MySqlParameter("@p_lastname", user.Lastname),
                            new MySqlParameter("@p_password", user.Password),
                            new MySqlParameter("@p_phone", user.PhoneNumber),
                            new MySqlParameter("@p_email", user.Email),
                            new MySqlParameter("@p_confirmationCode", user.ConfirmationCode));
            reader.Close();
            return GetUserInfo(user.Username);
        }
        public User RegisterManager(User user)
        {
            MySqlDataReader reader = GetStoredProcedureReader("RegisterManager",
                            new MySqlParameter("@p_username", user.Username),
                            new MySqlParameter("@p_firstname", user.Firstname),
                            new MySqlParameter("@p_lastname", user.Lastname),
                            new MySqlParameter("@p_password", user.Password),
                            new MySqlParameter("@p_phone", user.PhoneNumber),
                            new MySqlParameter("@p_email", user.Email),
                            new MySqlParameter("@p_groceryID", user.GroceryId),
                            new MySqlParameter("@p_confirmationCode", user.ConfirmationCode));
            reader.Close();
            return GetUserInfo(user.Username);
        }
        public List<User> DemoGetUserList()
        {
            MySqlDataReader reader = GetStoredProcedureReader("DemoGetUserList");
            
            List<User> users = new List<User>();
            while (reader.Read()){
                User user = new User();
                user.Username = ReadColumn(reader, "Username").ToString();
                user.Password = ReadColumn(reader, "PassW").ToString();
                user.Role = ReadColumn(reader, "UserType").ToString();
                users.Add(user);
            }
            return users;
        }
        public User GetUser(string username, string password)
        {
            MySqlDataReader reader = GetStoredProcedureReader("login", 
                            new MySqlParameter("@p_username", username),
                            new MySqlParameter("@p_password", password));
            reader.Read();
            User user = new User();
            user.Username = ReadColumn(reader, "Username").ToString();
            user.Firstname = ReadColumn(reader, "FirstName").ToString(); 
            user.Lastname = ReadColumn(reader, "LastName").ToString();
            user.Role = ReadColumn(reader, "UserType").ToString();
            user.Email = ReadColumn(reader, "Email").ToString();
            user.PhoneNumber = ReadColumn(reader, "Phone").ToString();
            user.GroceryId = (int)ReadColumn(reader, "DefaultStoreID");
            reader.Close();
            return user;
        }

        public User GetUserInfo(string username)
        {
            MySqlDataReader reader = GetStoredProcedureReader("GetUserInfo",
                            new MySqlParameter("@p_username", username));
            reader.Read();
            User user = new User();
            user.Username = ReadColumn(reader, "Username").ToString();
            user.Firstname = ReadColumn(reader, "FirstName").ToString();
            user.Lastname = ReadColumn(reader, "LastName").ToString();
            user.Role = ReadColumn(reader, "UserType").ToString();
            user.Email = ReadColumn(reader, "Email").ToString();
            user.PhoneNumber = ReadColumn(reader, "Phone").ToString();
            user.GroceryId = (int)ReadColumn(reader, "DefaultStoreID");
            // Buyer attributes
            if (user.Role != "buyer") return user;
            if (ReadColumn(reader, "DefaultPaymentId") == System.DBNull.Value) user.DefaultPaymentMethodId = null;
            else user.DefaultPaymentMethodId = (int)ReadColumn(reader, "DefaultPaymentId");

            user.AddressLine2 = ReadColumn(reader, "AddressLine2").ToString();
            user.StreetAddress = ReadColumn(reader, "Street").ToString();
            user.City = ReadColumn(reader, "City").ToString();
            user.State = ReadColumn(reader, "State").ToString();
            user.ZipCode = ReadColumn(reader, "ZipCode").ToString();
            // Buyer Payment methods
            reader.NextResult();
            user.PaymentMethods = new List<PaymentMethod>();
            while (reader.Read())
            {
                PaymentMethod paymentMethod = new PaymentMethod();
                if (ReadColumn(reader, "PaymentID") == System.DBNull.Value) return user;
                paymentMethod.Id = (int)ReadColumn(reader, "PaymentID");
                paymentMethod.IsDefault = paymentMethod.Id == user.DefaultPaymentMethodId;
                paymentMethod.Name = ReadColumn(reader, "PaymentName").ToString();
                paymentMethod.AccountNumber = ReadColumn(reader, "AccountNumber").ToString();
                paymentMethod.RoutineNumber = ReadColumn(reader, "RoutineNumber").ToString();
                user.PaymentMethods.Add(paymentMethod);
            }
            reader.Close();
            return user;

            
        }

        public User UpdateUserContactInfo(string username, string phoneNumber, string email)
        {
            MySqlDataReader reader = GetStoredProcedureReader("UpdateUserContactInfo",
                            new MySqlParameter("@p_username", username),
                            new MySqlParameter("@p_phone", phoneNumber),
                            new MySqlParameter("@p_email", email));
            reader.Close();
            return GetUserInfo(username);
        }

        public User UpdateUserAddressInfo(string username, string streetAddress, string addressLine2,
                                            string city, string state, string zipCode)
        {
            MySqlDataReader reader = GetStoredProcedureReader("UpdateUserAddressInfo",
                        new MySqlParameter("@p_username", username),
                        new MySqlParameter("@p_street", streetAddress),
                        new MySqlParameter("@p_line2", addressLine2),
                        new MySqlParameter("@p_city", city),
                        new MySqlParameter("@p_state", state),
                        new MySqlParameter("@p_zip", zipCode));
            reader.Close();
            return GetUserInfo(username);
        }


        public User AddPaymentMethod(string username, string name, string accountNumber, string routineNumber, bool isDefault)
        {
            MySqlDataReader reader = GetStoredProcedureReader("AddUserPaymentMethod",
                        new MySqlParameter("@p_username", username),
                        new MySqlParameter("@p_paymentname", name),
                        new MySqlParameter("@p_AccountNumber", accountNumber),
                        new MySqlParameter("@p_RoutineNumber", routineNumber),
                        new MySqlParameter("@p_isDefault", isDefault));
            reader.Close();
            return GetUserInfo(username);
        }
        public User UpdatePaymentMethod(string username, int paymentMethodId, string name, string accountNumber,
                                        string routineNumber, bool isDefault)
        {
            MySqlDataReader reader = GetStoredProcedureReader("UpdateUserPaymentMethod",
                         new MySqlParameter("@p_paymentID", paymentMethodId),
                         new MySqlParameter("@p_username", username),
                         new MySqlParameter("@p_paymentname", name),
                         new MySqlParameter("@p_AccountNumber", accountNumber),
                         new MySqlParameter("@p_RoutineNumber", routineNumber),
                         new MySqlParameter("@p_isDefault", isDefault));
            reader.Close();
            return GetUserInfo(username);
        }
        public User DeletePaymentMethod(string username, int paymentMethodId)
        {
            MySqlDataReader reader = GetStoredProcedureReader("DeleteUserPaymentMethod",
                         new MySqlParameter("@p_username", username),
                         new MySqlParameter("@p_paymentID", paymentMethodId));
            reader.Close();
            return GetUserInfo(username);
        }
        public void SwitchStore(string username, int groceryId)
        {

        }
    }
}
