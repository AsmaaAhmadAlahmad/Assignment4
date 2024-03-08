using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace Assignement3_Domain.Helper
{
    //  من أجل الكلاس الأصلي T
    // FileMngementMvcApp.Models  من أجل الكلاس الذي يُستخدم للإضافة والمتواجد في مجلد B
    // FileMngementMvcApp.Models  من أجل الكلاس الذي يُستخدم للتعديل والمتواجد في مجلد C

    public class HelperMapper<T, B, C>
    {
        public readonly Mapper MapperForCreate;
        public readonly Mapper MapperForUpdate;
     

        public HelperMapper()
        {
            MapperConfiguration configForCreate = new MapperConfiguration(cfg => cfg.CreateMap<B, T>());
            Mapper _mapperForCreate = new Mapper(configForCreate);
            MapperForCreate = _mapperForCreate;


            MapperConfiguration configForUpdate = new MapperConfiguration(cfg => cfg.CreateMap<C, T>());
            Mapper _mapperForUpdate = new Mapper(configForUpdate);
            MapperForUpdate = _mapperForUpdate;


            

        }


    }
}
