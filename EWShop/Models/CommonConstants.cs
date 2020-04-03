using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWShop.Models
{
    public static class CommonConstants
    {
        /* ========== String connection ========== */
        public const string CONNECTION_STRING = "MasterDataConnectionString";
        public const string CONNECTION_STRING_MWG_AQUA = "MWGAquaConnectionString";
        public const string SERVICEINFO_CONNECTION_STRING = "ServiceInfoConnectionString";
        public const string AEV_WMS_CONNECTION_STRING = "AEVWMSConnectionString";
        public const string HEV_WMS_CONNECTION_STRING = "HEVWMSConnectionString";
        public const string AMI_CONNECTION_STRING = "AMIConnectionString";
        public const string UPLOAD_DIRECT_STRING = "~/FileUploads/";
        public const string URL_SERVER = "http://ew.aquavietnam.com.vn/FileUploads/";
        public const string FIBO_URL = "http://center.fibosms.com/service.asmx/SendMaskedSMS?clientNo={0}&clientPass={1}&senderName={2}&phoneNumber={3}&smsMessage={4}&smsGUID=0&serviceType=0";
        public const string CLIENT_NO = "CL1609240012";
        public const string CLIENT_PASS = "AClCAqUaOu24tTnDVnNainMC892Ska2st";
        public const string SENDER_NAME = "AQUAVietNam";

        public const string SENDER_EMAIL = "cskh@aquavietnam.vn";
        public const string SENDER_PASS = "Aqua2019@1";
        public const string HOST_MAIL = "smtp.office365.com";

        public const string USERNAME_MWG = "aqua";
        public const string PASSWORD_MWG = "guqS5";

        public const int PORT = 587;
        /* ============= Variable ================ */
        public const string USER_INFO = "USER_INFO";
        public const string STATUS_INFO = "STATUS_INFO";
        public const string SHOP_INFO = "SHOP_INFO";
        public const string MATERIAL_INFO = "MATERIAL_INFO";
        public const string MAIN_MENUS_INFO = "MAIN_MENUS_INFO";
        public const string CHILD_MENUS_INFO = "CHILD_MENUS_INFO";
        public const string PLANTS_INFO = "PLANTS_INFO";
        public const string MODELS_INFO = "MODELS_INFO";
        public const string REGIONS_INFO = "REGIONS_INFO";
        public const string ROLES_INFO = "ROLES_INFO";
        public const string MINISTRY_INFO = "MINISTRY_INFO";
        public const string PROVINCES_INFO = "PROVINCES_INFO";
        public const string CHANNEL_TYPE_INFO = "CHANNEL_TYPE_INFO";
        public const string CHANNEL_GROUP_INFO = "CHANNEL_GROUP_INFO";
        public const string SHOP_GROUP_INFO = "SHOP_GROUP_INFO";
        public const string PROGRAM_INFO = "PROGRAM_INFO";
        public const string PREFIX_ID = "8077";
        public const string COMMAND_CODE = "AQUA";
        public const string AUTHORIZED_TOKEN = "AuthorizedToken";
        /* ========== String format ============== */
        public const string FORMAT_DATE_DDMMYYYY = "dd/MM/yyyy";
        /* ========== Result return ============== */
        public const string EXECUTE_SUCCESS = "1";
        public const string EXECUTE_NODATA = "2";
        public const string EXECUTE_UNSUCCESS = "0";
        /* ========== Stored procedure ============== */
        public const string STORED_PROCEDURE_GET_LIST_MATERIAL = "uspWeb_Material_View";
        public const string STORED_PROCEDURE_GET_MATERIAL = "uspWeb_Material_Select";
        public const string STORED_PROCEDURE_SAVE_MATERIAL = "uspWeb_Material_SaveChange";
        public const string STORED_PROCEDURE_INSERT_SELL_OUT_DAILY = "uspWeb_Barcode_SellOutDaily";
        public const string STORED_PROCEDURE_LIST_SELL_OUT_DAILY = "uspWeb_Barcode_SellOutDaily_List";
        public const string STORED_PROCEDURE_LIST_SELL_OUT_HISTORY = "uspWeb_Barcode_SellOutHistory_List";
        public const string STORED_PROCEDURE_GET_INFO_STATUS_BARCODE = "uspWeb_Barcode_GetInfoStatus";
        public const string STORED_PROCEDURE_LIST_SELL_OUT_HISTORY_FOR_KEYSHOP = "uspWeb_Barcode_SellOutHistory_KeyShop";
        public const string STORED_PROCEDURE_CHECK_BARCODE = "uspWeb_Barcode_CheckBarcodeInfo";
        public const string STORED_PROCEDURE_MODEL_FILTER = "uspWeb_Barcode_filterModel";
        public const string STORED_PROCEDURE_SHOP_FILTER = "uspWeb_Barcode_filterShop";
        public const string STORED_PROCEDURE_CHECK_PHONE_VALIDATE = "uspWeb_PhoneValidate_CheckPhone";
        public const string STORED_PROCEDURE_GET_LIST_USER = "uspWeb_User_View";
        public const string STORED_PROCEDURE_GET_USER_SELECT = "uspWeb_User_Select";
        public const string STORED_PROCEDURE_GET_INFO_USER = "uspWeb_User_GetInfomation";
        public const string STORED_PROCEDURE_LOGOUT = "uspWeb_User_Logout";
        public const string STORED_PROCEDURE_CHECK_USER_EXIST = "uspWeb_User_CheckUserExist";
        public const string STORED_PROCEDURE_UPDATE_TOKEN = "uspWeb_User_UpdateToken";
        public const string STORED_PROCEDURE_IS_AUTHORIZE = "uspWeb_User_IsAuthorize";
        public const string STORED_PROCEDURE_FIND_USER = "uspWeb_User_FindUser";
        public const string STORED_PROCEDURE_CHANGE_PASSWORD = "uspWeb_User_ChangePass";
        public const string STORED_PROCEDURE_SAVE_USER = "uspWeb_User_SaveChange";
        public const string STORED_PROCEDURE_GET_ROLES_FOR_USER = "uspWeb_User_GetRolesForUser";
        public const string STORED_PROCEDURE_GET_LIST_SHOP = "uspWeb_Channel_View";
        public const string STORED_PROCEDURE_GET_SHOP = "uspWeb_Channel_Select";
        public const string STORED_PROCEDURE_SAVE_SHOP = "uspWeb_Channel_SaveChange";
        public const string STORED_PROCEDURE_GET_LIST_MENU = "uspWeb_Menu_View";
        public const string STORED_PROCEDURE_GET_LIST_FILE_UPLOAD = "uspWeb_FileUpload_View";
        public const string STORED_PROCEDURE_GET_LIST_DATA_FILE_UPLOAD = "uspWeb_FileUpload_ViewData";
        public const string STORED_PROCEDURE_INSERT_FILE_UPLOAD = "uspWeb_FileUpload_Insert";
        public const string STORED_PROCEDURE_UPDATE_FILE_UPLOAD = "uspWeb_FileUpload_UpdateStatus";
        public const string STORED_PROCEDURE_EXECUTE_1ROW = "uspWeb_FileUpload_Execute1Row";
        public const string STORED_PROCEDURE_COMPLETED_1ROW = "uspWeb_FileUpload_Completed1Row";
        public const string STORED_PROCEDURE_FILE_UPLOAD_UPDATE = "uspWeb_FileUpload_UpdateStatus_Test";
        public const string STORED_PROCEDURE_CHECK_FILE_UPLOAD = "uspWeb_FileUpload_CheckStatus";
        public const string STORED_PROCEDURE_LOAD_INFO_FILE_UPLOAD = "uspWeb_FileUpload_LoadInfo";
        public const string STORED_PROCEDURE_LOAD_INFO_FILE_UPLOAD_TEST = "uspWeb_FileUpload_LoadInfoFileUpload";
        public const string STORED_PROCEDURE_CHECK_PHONE_SHOP = "uspWeb_EWarranty_CheckPhoneShop";
        public const string STORED_PROCEDURE_CHECK_STATUS_EW = "uspWeb_EWarranty_CheckStatus";
        public const string STORED_PROCEDURE_REGISTER_EW_SMS_CUSTOMER = "uspWeb_EWarranty_RegEWBySMSCustomer";
        public const string STORED_PROCEDURE_REGISTER_EW_SMS_SHOP = "uspWeb_EWarranty_RegEWBySMSShop";
        public const string STORED_PROCEDURE_REGISTER_EW_PSI = "uspWeb_EWarranty_RegEWByPSIWeb";
        public const string STORED_PROCEDURE_REGISTER_EW_WEB_CUSTOMER = "uspWeb_EWarranty_RegEWByWebCustomer";
        public const string STORED_PROCEDURE_SAVE_CONTENT_SMS = "uspWeb_SMS_SaveContent";
        public const string STORED_PROCEDURE_SERVICE_REQUEST_FOR_CUSTOMER = "uspWeb_ServiceRequest_InsertForCustomer";
        public const string STORED_PROCEDURE_SUPPORT_REQUEST = "uspWeb_SupportRequest_InsertForCustomer";
        public const string STORED_PROCEDURE_SERVICE_REQUEST_FOR_SHOP = "uspWeb_ServiceRequest_InsertForShop";
        public const string STORED_PROCEDURE_SERVICE_REQUEST_GET_ITEM = "uspWeb_ServiceRequest_GetItem";
        public const string STORED_PROCEDURE_SERVICE_REQUEST_UPDATE_STATUS = "uspWeb_ServiceRequest_UpdateStatusTransferGSIS";
        public const string STORED_PROCEDURE_SERVICE_REQUEST_LIST_WARD = "uspWeb_ServiceRequest_GetListWard";
        public const string STORED_PROCEDURE_SERVICE_REQUEST_GET_SRNUM = "uspWeb_ServiceRequest_GetSRNum";
        public const string STORED_PROCEDURE_SERVICE_REQUEST_LIST_EW_FOR_REG_SR = "uspWeb_ServiceRequest_ListEWForRegSR";
        public const string STORED_PROCEDURE_SERVICE_REQUEST_GET_INFO_SR_BY_EWARRANTY = "uspWeb_ServiceRequest_GetInfoByEwarranty";
        public const string STORED_PROCEDURE_SAVE_CHANGE_EWARRANTY = "uspWeb_EWarranty_SaveChangeItem";
        public const string STORED_PROCEDURE_SERVICE_REQUEST_GET_INFO_MINISTRY = "uspWeb_Survey_GetInfoMinistry";
        public const string STORED_PROCEDURE_SURVEY_QUESTIONS = "uspWeb_Survey_listQuestions";
        public const string STORED_PROCEDURE_SURVEY_ANSWERS = "uspWeb_Survey_listAnswers";
        public const string STORED_PROCEDURE_IMAGE_VIEW = "uspWeb_Image_View";
        public const string STORED_PROCEDURE_IMAGE_INSERT = "uspWeb_Image_Insert";
        public const string STORED_PROCEDURE_IMAGE_DELETE = "uspWeb_Image_Delete";
        /* ========== String sql ============== */
        public const string SQL_TEXT_GET_LIST_PROVINCE = "SELECT PROVINCEID AS VALUE,PROVINCENAME AS TEXT FROM TBLPROVINCE";
        public const string SQL_TEXT_GET_LIST_DISTRICTS = "SELECT DISTRICTID AS VALUE,DISTRICTVNNAME AS TEXT FROM TBLDISTRICT WHERE PROVINCEID=@PROVINCEID";
        public const string SQL_TEXT_GET_LIST_PLANTS = "SELECT PLANTID AS VALUE,PLANTNAMEVN AS TEXT FROM TBLPLANT";
        public const string SQL_TEXT_GET_LIST_CUSTOMERS = "select CUSTOMERCODE as VALUE, CUSTOMERCODE+' - '+CUSTOMERNAME as TEXT from TBLCUSTOMER";
        public const string SQL_TEXT_GET_LIST_MODELS_BY_PLANT = "SELECT MATID AS VALUE, ISNULL(MODELTYPE,MATDESCRIPTION) AS TEXT FROM vTBLMaterials WHERE ISNULL(@PLANTID,0)=0 OR PLANTID=@PLANTID ORDER BY MATID";
        public const string SQL_TEXT_GET_LIST_MODELS = "SELECT MODEL AS VALUE,MTFULLNAME AS TEXT FROM TBLMODEL";
        public const string SQL_TEXT_GET_LIST_MATERIALS = "SELECT MATID AS VALUE, MODELTYPE AS TEXT FROM TBLMATERIAL ORDER BY MODELTYPE";
        public const string SQL_TEXT_GET_LIST_REGIONS = "SELECT REGIONID AS [VALUE], RGNAME AS [TEXT] FROM TBLREGION";
        public const string SQL_TEXT_GET_LIST_ROLES = "SELECT ROLEID AS [VALUE], ROLESHORTNAME AS [TEXT] FROM TBLROLES";
        public const string SQL_TEXT_GET_LIST_CHANNEL_TYPES = "SELECT CTID AS VALUE,CTNAME AS TEXT FROM TBLCHANNELTYPE";
        public const string SQL_TEXT_GET_LIST_CHANNEL_GROUPS = "SELECT CGID AS VALUE,CGNAME AS TEXT FROM TBLCHANNELGROUP";
        public const string SQL_TEXT_GET_LIST_SHOP_GROUPS = "SELECT GROUPID AS VALUE,GROUPNAME AS TEXT FROM TBLSHOPGROUP";
        public const string SQL_TEXT_GET_LIST_CHANNELS = "SELECT CHANNELID AS VALUE,CHANNELCODE + ' - ' + CHANNELNAME AS TEXT FROM TBLCHANNEL";
        public const string SQL_TEXT_GET_URL_IMAGE = "SELECT IMGURL FROM TBLEWIMAGE WHERE FILEID=@FILEID AND IMGID=@IMGID";
        public const string SQL_TEXT_GET_LIST_ID_EXCEL = "SELECT * FROM vTMPBARCODE_EXCEL WHERE FILEID=@FILEID AND ISNULL(STATUSID,0)<>0 AND STATUS <> 'completed' AND ISNULL(STATUSID,0)<>4";
        public const string SQL_TEXT_GET_ITEM_EWARRANTY = "SELECT * FROM vTBLEWarrantyForKeyShop WHERE EWCARDID=@EWCARDID AND FLAT=@FLAT";
        public const string SQL_TEXT_GET_LIST_SUPPORTREQUESTTYPE = "SELECT SUPPORTRQID as TypeID, SUPPORTRQNAME as TypeName from TBLSUPPORTREQUESTTYPE";
        public const string SQL_TEXT_GET_LIST_SERVICE_REQUEST_NOT_TRANSFER_GSIS = "SELECT SRID FROM TBLSERVICEREQUEST WHERE ISNULL(GSIS_ERRCODE,1)=1 AND YEAR(ADDDATE)=@YEAR AND MONTH(ADDDATE)=@MONTH";
        public const string SQL_TEXT_GET_LIST_REWARD_STATUS = "SELECT RSID AS VALUE, RSVNNAME AS TEXT from TBLREWARDSTATUS";
        public const string SQL_TEXT_GET_LIST_PROGRAMS = "SELECT PROID AS VALUE, PRONAME AS TEXT from TBLPROGRAM WHERE ISACTIVE=1";
        /* ========== Menu ============== */
        public const string MENU_MAIN_BASIC_DATA = "BasicData";
        public const string MENU_MAIN_E_WARRANTY = "EWarranty";
        public const string MENU_MAIN_SELL = "Sell";
        public const string MENU_MAIN_SELL_THROUGH = "SellThrough";
        public const string MENU_USERS = "Users";
        public const string MENU_MATERIALS = "Materials";
        public const string MENU_SHOPS = "Shops";
        public const string MENU_VIEW_HISTORY = "ViewHistory";
        public const string MENU_CHECK_BARCODE = "CheckBarcode";
    }

}