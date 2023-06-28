using Game.Common;
using System;

namespace Game.Common
{
    public abstract class ACommonTypedView<TModel> : ACommonView where TModel : ICommonModel
    {
        protected TModel _model;

        public override void SetModel(ICommonModel model)
        {
            ClearSubscriptions();

            if (model is not TModel newModel)
            {
                throw new NotSupportedException();
            }

            _model = newModel;

            CreateSubscriptions();
            UpdateView(_model);
        }

        private void OnDestroy()
        {
            ClearSubscriptions();
        }

        protected abstract void UpdateView(TModel model);

        protected virtual void CreateSubscriptions()
        {
            if (_model == null)
            {
                return;
            }

            _model.OnModelChanged += HandleModelChanged;
        }

        protected virtual void ClearSubscriptions()
        {
            if (_model == null)
            {
                return;
            }

            _model.OnModelChanged -= HandleModelChanged;
        }

        protected virtual void HandleModelChanged()
        {
            UpdateView(_model);
        }
    }
}
