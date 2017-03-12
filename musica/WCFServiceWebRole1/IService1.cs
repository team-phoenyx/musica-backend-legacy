using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebGet(UriTemplate = "/create_room/{room_name}/{location}",
             RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json
         )]
        Message create_room(string room_name, string location);

        [OperationContract]
        [WebGet(UriTemplate = "/destory_room/{join_code}/{room_id}", RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json
         )]
        void destroy_room(string join_code, string room_id);

        [OperationContract]
        [WebGet(UriTemplate = "/join_room/{join_code}",
             RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json
         )]
        Message join_room(string join_code);

        [OperationContract]
        [WebGet(UriTemplate = "/get_location/{join_code}",
             RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json
         )]
        Message get_location(string join_code);

        [OperationContract]
        [WebGet(UriTemplate = "/close_current_round/{join_code}/{room_id}", RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json
         )]
        void close_current_round(string join_code, string room_id);

        [OperationContract]
        [WebGet(
             UriTemplate =
                 "/start_next_round/{join_code}/{room_id}/{song_choice1}/{song_choice2}/{song_choice3}/{song_choice4}/{image1}/{image2}/{image3}/{image4}/{artist1}/{artist2}/{artist3}/{artist4}",
             RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json
         )]
        Message start_next_round(string join_code, string room_id, string song_choice1, string song_choice2,
            string song_choice3, string song_choice4, string image1, string image2, string image3, string image4,
            string artist1, string artist2, string artist3, string artist4);

        [OperationContract]
        [WebGet(UriTemplate = "/vote/{join_code}/{choice_number}",
             RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json
         )]
        Message vote(string join_code, string choice_number);


        [OperationContract]
        [WebGet(UriTemplate = "/get_vote_status/{join_code}/{room_id}",
             RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json
         )]
        Message get_vote_status(string join_code, string room_id);

        [OperationContract]
        [WebGet(UriTemplate = "/get_choices/{join_code}",
             RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json
         )]
        Message get_choices(string join_code);
    }
}
