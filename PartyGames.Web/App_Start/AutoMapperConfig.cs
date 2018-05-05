using System;
using System.Text;
using AutoMapper;
using PartyGames.Data.BingoContext;
using PartyGames.Service.Bingo;
using PartyGames.Service.WebService.Models;
using PartyGames.Web.Models.Bingo;
using PartyGames.Web.Models.Mc;

namespace PartyGames.Web
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Player, PlayerModel>()
                    .ForMember(
                        dest => dest.CardNumbers,
                        opts => opts.MapFrom(src => src.GetNumberList())
                    )
                    .ForMember(
                        dest => dest.CardName,
                        opts => opts.MapFrom(src => "#" + src.CardUniqueNo + " " + src.Name.ToUpper())
                    );


                cfg.CreateMap<Game, GameModel>()
                    .ForMember(
                        dest => dest.RolledNumbers,
                        opts => opts.MapFrom(src => src.GetRolledNumberList())
                    );

                
                cfg.CreateMap<EposMcGame, McGameModel>();
                cfg.CreateMap<EposMcQuestion, McQuestionModel>();

                
            });
        }
    }

    public class AccmImagePathResolver : IMemberValueResolver<object, object, string, string>
    {
        public string Resolve(object source, object destination, string sourceMember, string destMember, ResolutionContext context)
        {
            //https://accm.ascotchang.com/sysimage/viewer?type=path&value=XFxISy1OQVMtMDJcaW1hZ2VzXENvdW50cnlGbGFnc1xTTS5Qbmc=

            if (string.IsNullOrWhiteSpace(sourceMember) || !sourceMember.StartsWith("//"))
                return sourceMember;

            var bytes = Encoding.UTF8.GetBytes(sourceMember);
            return "https://accm.ascotchang.com/sysimage/viewer?type=path&value=" + Convert.ToBase64String(bytes);

        }
    }
}