using System;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMapper.Tests
{
	[TestFixture]
	public class EnumMappingFixture
	{
		[SetUp]
		[TearDown]
		public void Cleanup()
		{
			Mapper.Reset();
		}

		[Test]
		public void ShouldMapSharedEnum()
		{
			Mapper.CreateMap<Order, OrderDto>();

			var order = new Order
				{
					Status = Status.InProgress
				};

			var dto = Mapper.Map<Order, OrderDto>(order);

			dto.Status.ShouldEqual(Status.InProgress);
		}

		[Test]
		public void ShouldMapEnumByMatchingNames()
		{
			Mapper.CreateMap<Order, OrderDtoWithOwnStatus>();

			var order = new Order
				{
					Status = Status.InProgress
				};

			var dto = Mapper.Map<Order, OrderDtoWithOwnStatus>(order);

			dto.Status.ShouldEqual(StatusForDto.InProgress);
		}

		[Test]
		public void ShouldMapEnumByMatchingValues()
		{
			Mapper.CreateMap<Order, OrderDtoWithOwnStatus>();

			var order = new Order
				{
					Status = Status.InProgress
				};

			var dto = Mapper.Map<Order, OrderDtoWithOwnStatus>(order);

			dto.Status.ShouldEqual(StatusForDto.InProgress);
		}

		[Test]
		public void ShouldMapEnumUsingCustomResolver()
		{
			Mapper.CreateMap<Order, OrderDtoWithOwnStatus>()
				.ForMember(dto => dto.Status, options => options
				                                         	.ResolveUsing<DtoStatusValueResolver>());

			var order = new Order
				{
					Status = Status.InProgress
				};

			var mappedDto = Mapper.Map<Order, OrderDtoWithOwnStatus>(order);

			mappedDto.Status.ShouldEqual(StatusForDto.InProgress);
		}

		[Test]
		public void ShouldMapEnumUsingGenericEnumResolver()
		{
			Mapper.CreateMap<Order, OrderDtoWithOwnStatus>()
				.ForMember(dto => dto.Status, options => options
				                                         	.ResolveUsing<EnumValueResolver<Status, StatusForDto>>()
				                                         	.FromMember(m => m.Status));

			var order = new Order
				{
					Status = Status.InProgress
				};

			var mappedDto = Mapper.Map<Order, OrderDtoWithOwnStatus>(order);

			mappedDto.Status.ShouldEqual(StatusForDto.InProgress);
		}

		public enum Status
		{
			InProgress = 1,
			Complete = 2
		}

		public enum StatusForDto
		{
			InProgress = 1,
			Complete = 2
		}

		public class Order
		{
			public Status Status { get; set; }
		}

		public class OrderDto
		{
			public Status Status { get; set; }
		}

		public class OrderDtoWithOwnStatus
		{
			public StatusForDto Status { get; set; }
		}

		public class DtoStatusValueResolver : IValueResolver
		{
			public object Resolve(object model)
			{
				return (StatusForDto) ((Order) model).Status;
			}

			public Type GetResolvedValueType()
			{
				return typeof (StatusForDto);
			}
		}

		public class EnumValueResolver<TInputEnum, TOutputEnum> : IValueResolver
		{
			public object Resolve(object model)
			{
				return (TOutputEnum) Enum.Parse(typeof (TOutputEnum), Enum.GetName(typeof (TInputEnum), model));
			}

			public Type GetResolvedValueType()
			{
				return typeof (TOutputEnum);
			}
		}
	}
}